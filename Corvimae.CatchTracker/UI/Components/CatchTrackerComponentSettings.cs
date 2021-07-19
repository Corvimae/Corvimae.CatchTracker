using Corvimae.CatchTracker.Component;
using Corvimae.CatchTracker.Properties;
using LiveSplit.Model;
using LiveSplit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using System.Xml;

namespace Corvimae.CatchTracker {
  public partial class CatchTrackerComponentSettings : UserControl {
    private const string SPECIES_DICTIONARY_DIRECTORY = "trackerDefinitions";
    private const string SPECIES_SPRITE_DIRECTORY = "sprites";

    private const string SETTING_OVERRIDE_TRACKER_FONT = "OverrideTrackerFont";
    private const string SETTING_TRACKER_FONT = "FontDefinition";
    private const string SETTING_BACKGROUND_COLOR = "BackgroundColor";
    private const string SETTING_DEFAULT_STATE_COLOR = "StateColorDefault";
    private const string SETTING_RIGHT_CLICK_STATE_COLOR = "StateColorRightClick";
    private const string SETTING_LEFT_CLICK_STATE_COLOR = "StateColorLeftClick";
    private const string SETTING_DOUBLE_CLICK_STATE_COLOR = "StateColorDoubleClick";
    private const string SETTING_TRACKER_WIDTH = "TrackerWidth";
    private const string SETTING_TRACKER_POSITION_X = "TrackerPositionX";
    private const string SETTING_TRACKER_POSITION_Y = "TrackerPositionY";
    private const string SETTING_COUNTER_WIDTH = "CounterWidth";
    private const string SETTING_COUNTER_POSITION_X = "CounterPositionX";
    private const string SETTING_COUNTER_POSITION_Y = "CounterPositionY";

    private const string SETTING_SPECIES_DICTIONARY = "SpeciesDictionaryFile";
    private const string SETTING_SPECIES_ITEMS = "SpeciesList";

    private const string SETTING_COUNTER_DEFINITIONS = "CounterDefinitions";

    private Dictionary<string, byte[]> BundledSpeciesDictionaries;

    public bool OverrideTrackerFont { get; set; }
    public Font TrackerFont;
    public Color BackgroundColor { get; set; }
    public int TrackerCellLength { get; set; }
    public int CounterCellWidth { get; set; }
    public int CounterCellHeight { get; set; }

    public Color DefaultStateColor { get; set; }
    public Color RightClickStateColor { get; set; }
    public Color LeftClickStateColor { get; set; }
    public Color DoubleClickStateColor { get; set; }

    public int TrackerXPosition { get; set; }
    public int TrackerYPosition { get; set; }

    public int CounterXPosition { get; set; }
    public int CounterYPosition { get; set; }

    public int RequestedTrackerWindowWidth { get; set; }
    public int RequestedCounterWindowWidth { get; set; }

    public int TrackerWindowWidth {
      get {
        return CalculateWindowWidth(RequestedTrackerWindowWidth, TrackerCellLength);
      }
    }
    public int TrackerWindowHeight {
      get {
        return CalculateWindowHeight(TrackerWindowWidth, TrackerCellLength,TrackerCellLength, VisibleSpecies.Count());
      }
    }

    public int CounterWindowWidth {
      get {
        return CalculateWindowWidth(RequestedCounterWindowWidth, CounterCellWidth);
      }
    }

    public int CounterWindowHeight {
      get {
        return CalculateWindowHeight(CounterWindowWidth, CounterCellWidth, CounterCellHeight, CounterDefinitions.Count());
      }
    }

    public string SpeciesDictionaryFile { get; set; } = "./resources/kanto.json";
    public SpeciesDictionary SpeciesDictionary = new SpeciesDictionary();
    public BindingList<CounterDefinition> CounterDefinitions;

    public BindingList<TrackedPokemonSpecies> TrackedSpecies;
    public IEnumerable<TrackedPokemonSpecies> VisibleSpecies {
      get {
        return TrackedSpecies?.Where(species => !species.Hidden) ?? new List<TrackedPokemonSpecies>();
      }
    }

    private LiveSplitState state;
    private CatchTrackerComponent trackerComponent;

    private OpenFileDialog openFileDialog1;
    private ColorDialog colorDialog1;
    private TableLayoutPanel tableLayoutPanel1;
    private GroupBox groupBox1;
    private TableLayoutPanel tableLayoutPanel2;
    private Label lblFont;
    private Button btnFont;
    private CheckBox chkFont;
    private Label label1;
    private Button btnBackgroundColor2;
    private GroupBox groupBox3;
    private GroupBox groupBox2;
    private TableLayoutPanel tableLayoutPanel3;
    private Button btnLeftClickStateColor;
    private Button btnRightClickStateColor;
    private Label label3;
    private Label label5;
    private Label label2;
    private Button btnDefaultStateColor;
    private Button btnDoubleClickStateColor;
    private Label label4;
    private TableLayoutPanel tableLayoutPanel4;
    private Label lblSpeciesDictionary;
    private Label label6;
    private Button btnSpeciesDictionary;
    private DataGridView dataGridView1;
    private System.ComponentModel.IContainer components;
    private BindingSource trackedPokemonSpeciesBindingSource;
    private DataGridViewTextBoxColumn dexNumberDataGridViewTextBoxColumn;
    private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
    private DataGridViewComboBoxColumn defaultStateDataGridViewTextBoxColumn;
    private DataGridViewCheckBoxColumn hiddenDataGridViewCheckBoxColumn;
    private GroupBox groupBox4;
    private DataGridView dataGridView2;
    private BindingSource counterDefinitionBindingSource;
    private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
    private DataGridViewCheckBoxColumn TrackDefaultState;
    private DataGridViewCheckBoxColumn TrackLeftClickState;
    private DataGridViewCheckBoxColumn TrackDoubleClickState;
    private DataGridViewCheckBoxColumn TrackRightClickState;
    private Label label11;

    public string TrackerFontString { get { return String.Format("{0} {1}", TrackerFont.FontFamily.Name, TrackerFont.Style); } }

    public CatchTrackerComponentSettings(LiveSplitState state, CatchTrackerComponent trackerComponent) {
      this.state = state;
      this.trackerComponent = trackerComponent;

      BundledSpeciesDictionaries = new Dictionary<string, byte[]> {
        ["kanto.json"] = Resources.kanto
      };

      SpeciesDictionaryFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SPECIES_DICTIONARY_DIRECTORY, "kanto.json");

      InitializeComponent();
      ResetToDefaults();

      CreateDictionaryFilesIfNotPresent();

      TrackedSpecies = new BindingList<TrackedPokemonSpecies>();
      CounterDefinitions = new BindingList<CounterDefinition>();


      trackedPokemonSpeciesBindingSource = new BindingSource(TrackedSpecies, null);
      counterDefinitionBindingSource = new BindingSource(CounterDefinitions, null);

      // Set bindings
      btnBackgroundColor2.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
      chkFont.DataBindings.Add("Checked", this, "OverrideTrackerFont", false, DataSourceUpdateMode.OnPropertyChanged);
      lblFont.DataBindings.Add("Text", this, "TrackerFontString", false, DataSourceUpdateMode.OnPropertyChanged);
      btnDefaultStateColor.DataBindings.Add("BackColor", this, "DefaultStateColor", false, DataSourceUpdateMode.OnPropertyChanged);
      btnRightClickStateColor.DataBindings.Add("BackColor", this, "RightClickStateColor", false, DataSourceUpdateMode.OnPropertyChanged);
      btnLeftClickStateColor.DataBindings.Add("BackColor", this, "LeftClickStateColor", false, DataSourceUpdateMode.OnPropertyChanged);
      btnDoubleClickStateColor.DataBindings.Add("BackColor", this, "DoubleClickStateColor", false, DataSourceUpdateMode.OnPropertyChanged);
      lblSpeciesDictionary.DataBindings.Add("Text", this, "SpeciesDictionaryFile", false, DataSourceUpdateMode.OnPropertyChanged);

      btnBackgroundColor2.Click += new System.EventHandler(this.ColorButtonClick);
      btnDefaultStateColor.Click += new System.EventHandler(this.ColorButtonClick);
      btnRightClickStateColor.Click += new System.EventHandler(this.ColorButtonClick);
      btnLeftClickStateColor.Click += new System.EventHandler(this.ColorButtonClick);
      btnDoubleClickStateColor.Click += new System.EventHandler(this.ColorButtonClick);

      btnSpeciesDictionary.Click += btnSpeciesDictionary_Click;
      chkFont.CheckedChanged += chkFont_CheckedChanged;

      Load += CatchTrackerComponentSettings_Load;

      openFileDialog1 = new OpenFileDialog() {
        Filter = "JSON files (*.json)|*.json",
        Title = "Select species dictionary file"
      };

      LoadSpeciesDictionary();
    }

    public void ResetToDefaults() {
      OverrideTrackerFont = false;
      TrackerFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);

      BackgroundColor = Color.FromArgb(255, 255, 255);

      TrackerCellLength = 40;
      CounterCellWidth = 150;
      CounterCellHeight = 80;

      DefaultStateColor = Color.FromArgb(255, 255, 255);
      RightClickStateColor = Color.FromArgb(0, 0, 255);
      LeftClickStateColor = Color.FromArgb(0, 0, 0);
      DoubleClickStateColor = Color.FromArgb(255, 0, 0);

      RequestedTrackerWindowWidth = 400;
    }

    public XmlNode GetSettings(XmlDocument document) {
      XmlElement settingsNode = document.CreateElement("Settings");
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_OVERRIDE_TRACKER_FONT, OverrideTrackerFont);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_TRACKER_FONT, TrackerFont);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_BACKGROUND_COLOR, BackgroundColor);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_DEFAULT_STATE_COLOR, DefaultStateColor);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_RIGHT_CLICK_STATE_COLOR, RightClickStateColor);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_LEFT_CLICK_STATE_COLOR, LeftClickStateColor);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_DOUBLE_CLICK_STATE_COLOR, DoubleClickStateColor);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_TRACKER_WIDTH, RequestedTrackerWindowWidth);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_TRACKER_POSITION_X, TrackerXPosition);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_TRACKER_POSITION_Y, TrackerYPosition);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_COUNTER_WIDTH, RequestedCounterWindowWidth);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_COUNTER_POSITION_X, CounterXPosition);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_COUNTER_POSITION_Y, CounterYPosition);
      SettingsHelper.CreateSetting(document, settingsNode, SETTING_SPECIES_DICTIONARY, SpeciesDictionaryFile);

      settingsNode.AppendChild(SerializeTrackedList(document));
      settingsNode.AppendChild(SerializeCounterDefinitions(document));

      return settingsNode;
    }

    public void SetSettings(XmlNode settings) {
      ResetToDefaults();
      XmlElement element = (XmlElement)settings;

      if (!element.IsEmpty) {
        if (element[SETTING_OVERRIDE_TRACKER_FONT] != null) {
          OverrideTrackerFont = SettingsHelper.ParseBool(element[SETTING_OVERRIDE_TRACKER_FONT]);
        }

        if (element[SETTING_TRACKER_FONT] != null) {
          TrackerFont = SettingsHelper.GetFontFromElement(element[SETTING_TRACKER_FONT]);
        }

        if (element[SETTING_BACKGROUND_COLOR] != null) {
          BackgroundColor = SettingsHelper.ParseColor(element[SETTING_BACKGROUND_COLOR]);
        }

        if (element[SETTING_DEFAULT_STATE_COLOR] != null) {
          DefaultStateColor = SettingsHelper.ParseColor(element[SETTING_DEFAULT_STATE_COLOR]);
        }

        if (element[SETTING_RIGHT_CLICK_STATE_COLOR] != null) {
          RightClickStateColor = SettingsHelper.ParseColor(element[SETTING_RIGHT_CLICK_STATE_COLOR]);
        }

        if (element[SETTING_LEFT_CLICK_STATE_COLOR] != null) {
          LeftClickStateColor = SettingsHelper.ParseColor(element[SETTING_LEFT_CLICK_STATE_COLOR]);
        }

        if (element[SETTING_DOUBLE_CLICK_STATE_COLOR] != null) {
          DoubleClickStateColor = SettingsHelper.ParseColor(element[SETTING_DOUBLE_CLICK_STATE_COLOR]);
        }

        if (element[SETTING_TRACKER_WIDTH] != null) {
          RequestedTrackerWindowWidth = SettingsHelper.ParseInt(element[SETTING_TRACKER_WIDTH]);
        }

        if (element[SETTING_TRACKER_POSITION_X] != null) {
          TrackerXPosition = SettingsHelper.ParseInt(element[SETTING_TRACKER_POSITION_X]);
        }

        if (element[SETTING_TRACKER_POSITION_Y] != null) {
          TrackerYPosition = SettingsHelper.ParseInt(element[SETTING_TRACKER_POSITION_Y]);
        }

        if (element[SETTING_COUNTER_WIDTH] != null) {
          RequestedCounterWindowWidth = SettingsHelper.ParseInt(element[SETTING_COUNTER_WIDTH]);
        }

        if (element[SETTING_COUNTER_POSITION_X] != null) {
          CounterXPosition = SettingsHelper.ParseInt(element[SETTING_COUNTER_POSITION_X]);
        }

        if (element[SETTING_COUNTER_POSITION_Y] != null) {
          CounterYPosition = SettingsHelper.ParseInt(element[SETTING_COUNTER_POSITION_Y]);
        }

        if (element[SETTING_SPECIES_DICTIONARY] != null) {
          SpeciesDictionaryFile = SettingsHelper.ParseString(element[SETTING_SPECIES_DICTIONARY]);
        }

        LoadSpeciesDictionary();

        TrackedSpecies.Clear();

        if (element[SETTING_SPECIES_ITEMS] != null) {
          foreach (XmlNode child in element[SETTING_SPECIES_ITEMS].GetElementsByTagName(TrackedPokemonSpecies.SETTING_KEY_SPECIES_ITEM)) {
            TrackedSpecies.Add(TrackedPokemonSpecies.Deserialize((XmlElement)child));
          }
        }

        CounterDefinitions.Clear();

        if (element[SETTING_COUNTER_DEFINITIONS] != null) {
          foreach (XmlNode child in element[SETTING_COUNTER_DEFINITIONS].GetElementsByTagName(CounterDefinition.SETTING_KEY_COUNTER_DEFINITION)) {
            CounterDefinitions.Add(CounterDefinition.Deserialize((XmlElement)child));
          }
        }

        RefreshDataViews();
      }
    }

    private XmlElement SerializeTrackedList(XmlDocument document) {
      XmlElement parent = SettingsHelper.ToElement(document, SETTING_SPECIES_ITEMS, (string)null);

      foreach (TrackedPokemonSpecies species in TrackedSpecies) {
        parent.AppendChild(species.Serialize(document));
      }

      return parent;
    }

    private XmlElement SerializeCounterDefinitions(XmlDocument document) {
      XmlElement parent = SettingsHelper.ToElement(document, SETTING_COUNTER_DEFINITIONS, (string)null);

      foreach (CounterDefinition counterDefinition in CounterDefinitions) {
        parent.AppendChild(counterDefinition.Serialize(document));
      }

      return parent;
    }

    private void CatchTrackerComponentSettings_Load(object sender, EventArgs e) {
      chkFont_CheckedChanged(null, null);
    }

    private void LoadSpeciesDictionary() {
      try {
        var loadedDictionary = JObject.Parse(File.ReadAllText(SpeciesDictionaryFile)).ToObject<SpeciesDictionary>();

        loadedDictionary.Filename = SpeciesDictionaryFile;

        if (loadedDictionary.Filename != SpeciesDictionary.Filename) {
          SpeciesDictionary = loadedDictionary;

          TrackedSpecies.Clear();
          foreach (var species in SpeciesDictionary.Pokemon) {
            TrackedSpecies.Add(new TrackedPokemonSpecies(species));
          }

          RefreshDataViews();
        }
      } catch (Exception ex) {
        MessageBox.Show($"Invalid species dictionary.\n\nError message: {ex.Message}\n\nDetails:\n\n{ex.StackTrace}");
      }
    }

    private void RefreshDataViews() {
      dataGridView1.DataSource = trackedPokemonSpeciesBindingSource;
      dataGridView1.Update();
      dataGridView2.DataSource = counterDefinitionBindingSource;
      dataGridView2.Update();
    }

    private int CalculateWindowWidth(int requestedWidth, int cellLength) {
      return Math.Max(1 * cellLength, requestedWidth - (requestedWidth % cellLength));
    }

    private int CalculateWindowHeight(int windowWidth, int cellWidth, int cellHeight, int cellCount) {
      var cellsPerRow = Math.Floor(1f * windowWidth / cellWidth);
      var maxRows = Math.Ceiling(1f * cellCount / cellsPerRow);

      return (int)Math.Ceiling(maxRows * cellHeight);
    }


    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.colorDialog1 = new System.Windows.Forms.ColorDialog();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.lblSpeciesDictionary = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.btnSpeciesDictionary = new System.Windows.Forms.Button();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.dexNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.defaultStateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.hiddenDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.trackedPokemonSpeciesBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnLeftClickStateColor = new System.Windows.Forms.Button();
      this.btnRightClickStateColor = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.btnDefaultStateColor = new System.Windows.Forms.Button();
      this.btnDoubleClickStateColor = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.lblFont = new System.Windows.Forms.Label();
      this.btnFont = new System.Windows.Forms.Button();
      this.chkFont = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.dataGridView2 = new System.Windows.Forms.DataGridView();
      this.counterDefinitionBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.btnBackgroundColor2 = new System.Windows.Forms.Button();
      this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.TrackDefaultState = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.TrackLeftClickState = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.TrackDoubleClickState = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.TrackRightClickState = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackedPokemonSpeciesBindingSource)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.counterDefinitionBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 151F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label11, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox4, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnBackgroundColor2, 1, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 142F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(456, 800);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox3, 4);
      this.groupBox3.Controls.Add(this.tableLayoutPanel4);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox3.Location = new System.Drawing.Point(3, 456);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(450, 341);
      this.groupBox3.TabIndex = 45;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Tracked Pokémon ";
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 3;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel4.Controls.Add(this.lblSpeciesDictionary, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.label6, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSpeciesDictionary, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.dataGridView1, 0, 1);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(444, 322);
      this.tableLayoutPanel4.TabIndex = 0;
      // 
      // lblSpeciesDictionary
      // 
      this.lblSpeciesDictionary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.lblSpeciesDictionary.AutoSize = true;
      this.lblSpeciesDictionary.Location = new System.Drawing.Point(148, 8);
      this.lblSpeciesDictionary.Name = "lblSpeciesDictionary";
      this.lblSpeciesDictionary.Size = new System.Drawing.Size(212, 13);
      this.lblSpeciesDictionary.TabIndex = 18;
      this.lblSpeciesDictionary.Text = "File";
      // 
      // label6
      // 
      this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(3, 8);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(139, 13);
      this.label6.TabIndex = 11;
      this.label6.Text = "Species Dictionary:";
      // 
      // btnSpeciesDictionary
      // 
      this.btnSpeciesDictionary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSpeciesDictionary.Location = new System.Drawing.Point(366, 3);
      this.btnSpeciesDictionary.Name = "btnSpeciesDictionary";
      this.btnSpeciesDictionary.Size = new System.Drawing.Size(75, 23);
      this.btnSpeciesDictionary.TabIndex = 19;
      this.btnSpeciesDictionary.Text = "Choose...";
      this.btnSpeciesDictionary.UseVisualStyleBackColor = true;
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.AutoGenerateColumns = false;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dexNumberDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.defaultStateDataGridViewTextBoxColumn,
            this.hiddenDataGridViewCheckBoxColumn});
      this.tableLayoutPanel4.SetColumnSpan(this.dataGridView1, 3);
      this.dataGridView1.DataSource = this.trackedPokemonSpeciesBindingSource;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(3, 32);
      this.dataGridView1.MultiSelect = false;
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.Size = new System.Drawing.Size(438, 287);
      this.dataGridView1.TabIndex = 20;
      // 
      // dexNumberDataGridViewTextBoxColumn
      // 
      this.dexNumberDataGridViewTextBoxColumn.DataPropertyName = "DexNumber";
      this.dexNumberDataGridViewTextBoxColumn.HeaderText = "No.";
      this.dexNumberDataGridViewTextBoxColumn.Name = "dexNumberDataGridViewTextBoxColumn";
      this.dexNumberDataGridViewTextBoxColumn.Width = 50;
      // 
      // nameDataGridViewTextBoxColumn
      // 
      this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
      this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
      this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
      // 
      // defaultStateDataGridViewTextBoxColumn
      // 
      this.defaultStateDataGridViewTextBoxColumn.DataPropertyName = "DefaultState";
      this.defaultStateDataGridViewTextBoxColumn.DataSource = new Corvimae.CatchTracker.TrackerState[] {
        Corvimae.CatchTracker.TrackerState.DEFAULT,
        Corvimae.CatchTracker.TrackerState.LEFT_CLICK,
        Corvimae.CatchTracker.TrackerState.DOUBLE_CLICK,
        Corvimae.CatchTracker.TrackerState.RIGHT_CLICK};
      this.defaultStateDataGridViewTextBoxColumn.HeaderText = "Default State";
      this.defaultStateDataGridViewTextBoxColumn.Name = "defaultStateDataGridViewTextBoxColumn";
      this.defaultStateDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
      // 
      // hiddenDataGridViewCheckBoxColumn
      // 
      this.hiddenDataGridViewCheckBoxColumn.DataPropertyName = "Hidden";
      this.hiddenDataGridViewCheckBoxColumn.HeaderText = "Hidden";
      this.hiddenDataGridViewCheckBoxColumn.Name = "hiddenDataGridViewCheckBoxColumn";
      // 
      // trackedPokemonSpeciesBindingSource
      // 
      this.trackedPokemonSpeciesBindingSource.DataSource = typeof(Corvimae.CatchTracker.TrackedPokemonSpecies);
      // 
      // groupBox2
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox2, 4);
      this.groupBox2.Controls.Add(this.tableLayoutPanel3);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(3, 114);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(450, 136);
      this.groupBox2.TabIndex = 44;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Tracker Colors";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.44444F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.55556F));
      this.tableLayoutPanel3.Controls.Add(this.btnLeftClickStateColor, 1, 1);
      this.tableLayoutPanel3.Controls.Add(this.btnRightClickStateColor, 1, 2);
      this.tableLayoutPanel3.Controls.Add(this.label3, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.label5, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.label2, 0, 2);
      this.tableLayoutPanel3.Controls.Add(this.btnDefaultStateColor, 1, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnDoubleClickStateColor, 1, 3);
      this.tableLayoutPanel3.Controls.Add(this.label4, 0, 3);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 5;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(444, 117);
      this.tableLayoutPanel3.TabIndex = 0;
      // 
      // btnLeftClickStateColor
      // 
      this.btnLeftClickStateColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.btnLeftClickStateColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btnLeftClickStateColor.Location = new System.Drawing.Point(147, 32);
      this.btnLeftClickStateColor.Name = "btnLeftClickStateColor";
      this.btnLeftClickStateColor.Size = new System.Drawing.Size(23, 23);
      this.btnLeftClickStateColor.TabIndex = 1;
      this.btnLeftClickStateColor.UseVisualStyleBackColor = false;
      // 
      // btnRightClickStateColor
      // 
      this.btnRightClickStateColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.btnRightClickStateColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btnRightClickStateColor.Location = new System.Drawing.Point(147, 61);
      this.btnRightClickStateColor.Name = "btnRightClickStateColor";
      this.btnRightClickStateColor.Size = new System.Drawing.Size(23, 23);
      this.btnRightClickStateColor.TabIndex = 13;
      this.btnRightClickStateColor.UseVisualStyleBackColor = false;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 8);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(138, 13);
      this.label3.TabIndex = 11;
      this.label3.Text = "Default State:";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 37);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(82, 13);
      this.label5.TabIndex = 12;
      this.label5.Text = "Left Click State:";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 66);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(89, 13);
      this.label2.TabIndex = 14;
      this.label2.Text = "Right Click State:";
      // 
      // btnDefaultStateColor
      // 
      this.btnDefaultStateColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.btnDefaultStateColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btnDefaultStateColor.Location = new System.Drawing.Point(147, 3);
      this.btnDefaultStateColor.Name = "btnDefaultStateColor";
      this.btnDefaultStateColor.Size = new System.Drawing.Size(23, 23);
      this.btnDefaultStateColor.TabIndex = 15;
      this.btnDefaultStateColor.UseVisualStyleBackColor = false;
      // 
      // btnDoubleClickStateColor
      // 
      this.btnDoubleClickStateColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.btnDoubleClickStateColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btnDoubleClickStateColor.Location = new System.Drawing.Point(147, 90);
      this.btnDoubleClickStateColor.Name = "btnDoubleClickStateColor";
      this.btnDoubleClickStateColor.Size = new System.Drawing.Size(23, 23);
      this.btnDoubleClickStateColor.TabIndex = 17;
      this.btnDoubleClickStateColor.UseVisualStyleBackColor = false;
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 95);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(98, 13);
      this.label4.TabIndex = 16;
      this.label4.Text = "Double Click State:";
      // 
      // groupBox1
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 4);
      this.groupBox1.Controls.Add(this.tableLayoutPanel2);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Location = new System.Drawing.Point(3, 32);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(450, 76);
      this.groupBox1.TabIndex = 43;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Tracker Font";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel2.Controls.Add(this.lblFont, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.btnFont, 2, 1);
      this.tableLayoutPanel2.Controls.Add(this.chkFont, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 3;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(444, 57);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // lblFont
      // 
      this.lblFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.lblFont.AutoSize = true;
      this.lblFont.Location = new System.Drawing.Point(148, 37);
      this.lblFont.Name = "lblFont";
      this.lblFont.Size = new System.Drawing.Size(212, 13);
      this.lblFont.TabIndex = 4;
      this.lblFont.Text = "Font";
      // 
      // btnFont
      // 
      this.btnFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.btnFont.Location = new System.Drawing.Point(366, 32);
      this.btnFont.Name = "btnFont";
      this.btnFont.Size = new System.Drawing.Size(75, 23);
      this.btnFont.TabIndex = 1;
      this.btnFont.Text = "Choose...";
      this.btnFont.UseVisualStyleBackColor = true;
      this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
      // 
      // chkFont
      // 
      this.chkFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.chkFont.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.chkFont, 2);
      this.chkFont.Location = new System.Drawing.Point(7, 6);
      this.chkFont.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
      this.chkFont.Name = "chkFont";
      this.chkFont.Size = new System.Drawing.Size(353, 17);
      this.chkFont.TabIndex = 0;
      this.chkFont.Text = "Override Layout Settings";
      this.chkFont.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 37);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(139, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Font:";
      // 
      // label11
      // 
      this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(3, 8);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(145, 13);
      this.label11.TabIndex = 40;
      this.label11.Text = "Background Color:";
      // 
      // groupBox4
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox4, 4);
      this.groupBox4.Controls.Add(this.dataGridView2);
      this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox4.Location = new System.Drawing.Point(3, 256);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(450, 194);
      this.groupBox4.TabIndex = 46;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Counters";
      // 
      // dataGridView2
      // 
      this.dataGridView2.AutoGenerateColumns = false;
      this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn1,
            this.TrackDefaultState,
            this.TrackLeftClickState,
            this.TrackDoubleClickState,
            this.TrackRightClickState});
      this.dataGridView2.DataSource = this.counterDefinitionBindingSource;
      this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView2.Location = new System.Drawing.Point(3, 16);
      this.dataGridView2.Name = "dataGridView2";
      this.dataGridView2.RowHeadersVisible = false;
      this.dataGridView2.Size = new System.Drawing.Size(444, 175);
      this.dataGridView2.TabIndex = 0;
      // 
      // counterDefinitionBindingSource
      // 
      this.counterDefinitionBindingSource.DataSource = typeof(Corvimae.CatchTracker.CounterDefinition);
      // 
      // btnBackgroundColor2
      // 
      this.btnBackgroundColor2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBackgroundColor2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btnBackgroundColor2.Location = new System.Drawing.Point(154, 3);
      this.btnBackgroundColor2.Name = "btnBackgroundColor2";
      this.btnBackgroundColor2.Size = new System.Drawing.Size(23, 23);
      this.btnBackgroundColor2.TabIndex = 41;
      this.btnBackgroundColor2.UseVisualStyleBackColor = false;
      // 
      // nameDataGridViewTextBoxColumn1
      // 
      this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
      this.nameDataGridViewTextBoxColumn1.HeaderText = "Name";
      this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
      // 
      // TrackDefaultState
      // 
      this.TrackDefaultState.DataPropertyName = "TrackDefaultState";
      this.TrackDefaultState.HeaderText = "Default";
      this.TrackDefaultState.Name = "TrackDefaultState";
      this.TrackDefaultState.Width = 80;
      // 
      // TrackLeftClickState
      // 
      this.TrackLeftClickState.DataPropertyName = "TrackLeftClickState";
      this.TrackLeftClickState.HeaderText = "Left Click";
      this.TrackLeftClickState.Name = "TrackLeftClickState";
      this.TrackLeftClickState.Width = 80;
      // 
      // TrackDoubleClickState
      // 
      this.TrackDoubleClickState.DataPropertyName = "TrackDoubleClickState";
      this.TrackDoubleClickState.HeaderText = "Double Click";
      this.TrackDoubleClickState.Name = "TrackDoubleClickState";
      this.TrackDoubleClickState.Width = 80;
      // 
      // TrackRightClickState
      // 
      this.TrackRightClickState.DataPropertyName = "TrackRightClickState";
      this.TrackRightClickState.HeaderText = "Right Click";
      this.TrackRightClickState.Name = "TrackRightClickState";
      this.TrackRightClickState.Width = 80;
      // 
      // CatchTrackerComponentSettings
      // 
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "CatchTrackerComponentSettings";
      this.Size = new System.Drawing.Size(456, 800);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackedPokemonSpeciesBindingSource)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.counterDefinitionBindingSource)).EndInit();
      this.ResumeLayout(false);

    }

    private void ColorButtonClick(object sender, EventArgs e) {
      SettingsHelper.ColorButtonClick((Button)sender, this);
    }

    private void btnFont_Click(object sender, System.EventArgs e) {
      var dialog = SettingsHelper.GetFontDialog(TrackerFont, 7, 20);
      dialog.FontChanged += (s, ev) => TrackerFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
      dialog.ShowDialog(this);
      lblFont.Text = TrackerFontString;
    }

    private void btnSpeciesDictionary_Click(object sender, System.EventArgs e) {
      if (openFileDialog1.ShowDialog() == DialogResult.OK) {
        try {
          SpeciesDictionaryFile = openFileDialog1.FileName;
          lblSpeciesDictionary.Text = SpeciesDictionaryFile;
          LoadSpeciesDictionary();
        } catch (SecurityException ex) {
          MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\nDetails:\n\n{ex.StackTrace}");
        }
      }
    }

    private void CreateDictionaryFilesIfNotPresent() {
      try {
        var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var definitionDirectory = Path.Combine(baseDirectory, SPECIES_DICTIONARY_DIRECTORY);
        var spriteDirectory = Path.Combine(definitionDirectory, SPECIES_SPRITE_DIRECTORY);

        if (!Directory.Exists(definitionDirectory)) {
          Directory.CreateDirectory(definitionDirectory);
        }

        if (!Directory.Exists(spriteDirectory)) {
          Directory.CreateDirectory(spriteDirectory);
        }

        foreach (var entry in BundledSpeciesDictionaries) {
          var path = Path.Combine(definitionDirectory, entry.Key);

          if (!File.Exists(path)) File.WriteAllBytes(path, entry.Value);
        }

        var spriteResources = Resources.ResourceManager
          .GetResourceSet(CultureInfo.CurrentCulture, false, true)
          .Cast<DictionaryEntry>()
          .Where(entry => entry.Value.GetType() == typeof(Bitmap));

        foreach (DictionaryEntry entry in spriteResources) {
          var resourceName = (string)entry.Key;

          if (resourceName.StartsWith("_")) resourceName = resourceName.Substring(1);

          var path = Path.Combine(spriteDirectory, $"{resourceName}.png");

          if (!File.Exists(path)) {
            using (MemoryStream memory = new MemoryStream()) {
              using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite)) {
                var bitmap = (Bitmap)entry.Value;

                bitmap.Save(memory, ImageFormat.Png);
                byte[] bytes = memory.ToArray();
                fs.Write(bytes, 0, bytes.Length);
              }
            }
          }
        }
      } catch (Exception ex) {
        MessageBox.Show($"Unable to build species dictionaries.\n\nError message: {ex.Message}\n\nDetails:\n\n{ex.StackTrace}");
      }
    }

    void chkFont_CheckedChanged(object sender, EventArgs e) {
      label1.Enabled = lblFont.Enabled = btnFont.Enabled = chkFont.Checked;
    }
  }
}
