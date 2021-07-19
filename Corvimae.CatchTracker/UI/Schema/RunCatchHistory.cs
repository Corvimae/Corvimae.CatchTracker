using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Corvimae.CatchTracker {
  public class CatchHistoryEvent {
    public int DexNumber { get; set; }
    public string Name { get; set; }
    public TrackerState State { get; set; }
    public TimeSpan? RealTime { get; set; }
    public TimeSpan? GameTime { get; set; }

    public CatchHistoryEvent(int dexNumber, string name, TrackerState state, TimeSpan? realTime, TimeSpan? gameTime) {
      DexNumber = dexNumber;
      Name = name;
      State = state;
      RealTime = realTime;
      GameTime = gameTime;
    }


    public XmlElement Serialize(XmlDocument document) {
      var element = document.CreateElement("CatchEvent");

      element.SetAttribute("dexNumber", DexNumber.ToString());
      element.SetAttribute("name", Name);
      element.SetAttribute("state", State.ToString());

      if (RealTime != null) {
        var realTimeElement = document.CreateElement("RealTime");

        realTimeElement.InnerText = RealTime.ToString();
        element.AppendChild(realTimeElement);
      }

      if (GameTime != null) {
        var realTimeElement = document.CreateElement("RealTime");

        realTimeElement.InnerText = RealTime.ToString();
        element.AppendChild(realTimeElement);
      }

      return element;
    }

    public static CatchHistoryEvent Deserialize(XmlElement element) {
      var dexNumber = int.Parse(element.GetAttribute("dexNumber"), CultureInfo.InvariantCulture);
      var name = element.GetAttribute("name");
      var state = (TrackerState)Enum.Parse(typeof(TrackerState), element.GetAttribute("state"));

      TimeSpan? realTime = null;
      TimeSpan? gameTime = null;

      if (element.GetElementsByTagName("RealTime").Count > 0) {
        TimeSpan x;

        if (TimeSpan.TryParse(element["RealTime"].InnerText, out x)) realTime = x;
      }

      if (element.GetElementsByTagName("GameTime").Count > 0) {
        TimeSpan x;

        if (TimeSpan.TryParse(element["GameTime"].InnerText, out x)) gameTime = x;
      }

      return new CatchHistoryEvent(
        dexNumber: dexNumber,
        name: name,
        state: state,
        realTime: realTime,
        gameTime: gameTime
      );
    }
  }

  public class CatchHistoryRun {
    public int RunId { get; set; }
    public List<CatchHistoryEvent> Events { get; set; }

    public CatchHistoryRun(int runId) {
      RunId = runId;
      Events = new List<CatchHistoryEvent>();
    }

    public CatchHistoryRun(int runId, List<CatchHistoryEvent> events) {
      RunId = runId;
      Events = events;
    }

    public XmlElement Serialize(XmlDocument document) {
      var element = document.CreateElement("Attempt");

      element.SetAttribute("id", RunId.ToString());

      foreach (var evt in Events) {
        element.AppendChild(evt.Serialize(document));
      }

      return element;
    }

    public static CatchHistoryRun Deserialize(XmlElement element) {
      return new CatchHistoryRun(
        runId: int.Parse(element.GetAttribute("id"), CultureInfo.InvariantCulture),
        events: element.GetElementsByTagName("CatchEvent").Cast<XmlElement>().Select(node => CatchHistoryEvent.Deserialize(node)).ToList()
      );
    }
  }


  public class CatchHistorySegment {
    public string Name { get; set; }
    public List<CatchHistoryRun> SegmentHistory { get; set; }

    public CatchHistorySegment(string name) {
      Name = name;
      SegmentHistory = new List<CatchHistoryRun>();
    }

    public CatchHistorySegment(string name, List<CatchHistoryRun> segmentHistory) {
      Name = name;
      SegmentHistory = segmentHistory;
    }

    public XmlElement Serialize(XmlDocument document) {
      var element = document.CreateElement("Segment");
      var nameElement = document.CreateElement("Name");
      var segmentHistoryElement = document.CreateElement("SegmentHistory");

      nameElement.InnerText = Name;

      foreach (var segment in SegmentHistory) {
        segmentHistoryElement.AppendChild(segment.Serialize(document));
      }

      element.AppendChild(nameElement);
      element.AppendChild(segmentHistoryElement);

      return element;
    }

    public static CatchHistorySegment Deserialize(XmlElement element) {
      return new CatchHistorySegment(
        name: element["Name"]?.InnerText,
        segmentHistory: element["SegmentHistory"]?.GetElementsByTagName("Attempt").Cast<XmlElement>().Select(node => CatchHistoryRun.Deserialize(node)).ToList() ?? new List<CatchHistoryRun>()
      );
    }
  }

  public class CatchHistory {
    public List<CatchHistorySegment> Segments { get; set; } = new List<CatchHistorySegment>();

    public void AddEvent(int runId, string splitName, TrackedPokemonSpecies species, TrackerState trackerState, Time time) {
      var relevantSegment = Segments.Where(segment => segment.Name == splitName).FirstOrDefault();

      if (relevantSegment == null) {
        relevantSegment = new CatchHistorySegment(splitName);

        Segments.Add(relevantSegment);
      }

      var relevantAttempt = relevantSegment.SegmentHistory.Where(attempt => attempt.RunId == runId).FirstOrDefault();

      if (relevantAttempt == null) {
        relevantAttempt = new CatchHistoryRun(runId);

        relevantSegment.SegmentHistory.Add(relevantAttempt);
      }

      relevantAttempt.Events.Add(new CatchHistoryEvent(
        species.DexNumber,
        species.Name,
        trackerState,
        time.RealTime,
        time.GameTime
      ));
    }

    public XmlElement Serialize(XmlDocument document) {
      var element = document.CreateElement("Segments");

      foreach (var segment in Segments) {
        element.AppendChild(segment.Serialize(document));
      }

      return element;
    }

    public static CatchHistory Deserialize(XmlElement element) {
      return new CatchHistory {
        Segments = element["Segments"]?.GetElementsByTagName("Segment").Cast<XmlElement>().Select(node => CatchHistorySegment.Deserialize(node)).ToList() ?? new List<CatchHistorySegment>()
      };
    }
  }
}
