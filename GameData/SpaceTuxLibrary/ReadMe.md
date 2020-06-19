This is a library of several useful functions which I find I use multiple times.  By putting them into
individual files, it's easy to only include those modules needed

There are currently three sets of functonality 

KSP_Log				A simple set of logging methods

KSP_ColorPicker		Provides a color picker for use by mods. Comes with 5 different
					textures the user can switch between to find one he/she prefers

KSP_PartHighlighter	Provides a way to add highlighting to parts on vessels


Usage
======

KSP_Log
	 public enum LEVEL { OFF = 0, ERROR = 1, WARNING = 2, INFO = 3, DETAIL = 4, TRACE = 5 };

	 public Log(string title)			Instantiates a new log instance, title is used in the log lines
	 public void setTitle(string t)		If you need to change the title
	 public LEVEL GetLevel()			Get current logging level
	 public void SetLevel(LEVEL level)
	 public LEVEL GetLogLevel()			same as GetLevel()
	 private bool IsLevel(LEVEL level)
	 public bool IsLogable(LEVEL level)

	 The following all write data to the log file if the logging level permits

	 public void Trace(String msg)		
	 public void Detail(String msg)
	 [ConditionalAttribute("DEBUG") public void Info(String msg)	These two are only active when compiled
	 [ConditionalAttribute("DEBUG")] public void Test(String msg)	in a debug mode
	 public void Warning(String msg)
	 public void Error(String msg)
	 public void Exception(Exception e)

KSP_ColorPicker

	This mod provides a simple ColorPicker for a mod.  It doesn't do anything by 
	itself, it is a tool to be used by another mod.

	This was originally written for use in the OSE Workshop, and is use by 
	both OSE Workshop and the PWB Fuel Balander mods

	Note that there is a Unity class called ColorPicker.  In order to differentiate
	this mod, it's called KSP_ColorPicker

	 public static bool showPicker { get; };
     public static bool success { get; }; 
	 public static Color SelectedColor {  get; };

	 public void PingTime()
	 public static KSP_ColorPicker CreateColorPicker(Color initialColor, string texturePath, bool activePingsNeeded = true)
	 public static KSP_ColorPicker CreateColorPicker(Color initialColor, string texturePath = null, bool activePingsNeeded = true, bool destroyOnClose = true)
	 public static KSP_ColorPicker CreateColorPicker(Color initialColor, bool useDefinedPosition = false, string texturePath = null,
            int pLeft = 0, int pTop = 0, bool activePingsNeeded = true, bool destroyOnClose = true)
     public bool UseKSPskin { private get; set; } = true;

KSP_PartHighlighter
	 A small minimod which will highlight specified parts in a flashing pattern.  Speed is configurable

	  public static PartHighlighter CreatePartHighlighter()
	  public int CreateHighlightList(float interval, Color c)
      public int CreateHighlightList(float interval = 1f)
	  public bool DestroyHighlightList(int id)
	  public bool SetHighlighting(int id, bool active)
	  public bool HighlightSinglePart(Color highlightC, Color edgeHighlightColor, Part p)
	  public bool AddPartToHighlight(int id, Part p)
	  public bool DisablePartHighlighting(int id, Part part)
	  public bool UpdateHighlightColors(int id, Color newHighlightColor)

