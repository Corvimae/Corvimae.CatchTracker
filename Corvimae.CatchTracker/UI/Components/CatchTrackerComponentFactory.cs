using Corvimae.CatchTracker.Component;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

[assembly: ComponentFactory(typeof(CatchTrackerComponentFactory))]

namespace Corvimae.CatchTracker.Component {
  public class CatchTrackerComponentFactory : IComponentFactory {
    public string ComponentName => "Catch Tracker";
    
    public string Description => "A catch tracker for Pokémon speedruns.";
    
    public ComponentCategory Category => ComponentCategory.Other;

    public IComponent Create(LiveSplitState state) => new CatchTrackerComponent(state);

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.Corvimae.CatchTracker.xml"; // todo

    public string UpdateURL => "http://livesplit.org/update/"; // todo

    public Version Version => Version.Parse("1.8.0");
  }
}
