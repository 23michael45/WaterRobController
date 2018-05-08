GUI Console 
by Nicholas Ventimiglia
-  AvariceOnline.com

The GUIConsole for unity3d is a Free Unity3d package that gives you the power 
and simplicity of a .Net Console application. Great for testing services and debugging.

This includes a text input and toolbar on the top, a scrolling console output below,
and a customizable command button menu to the right.

    Free Unity3d Package
    Clean C# code
    MvvM design (Data-model != view-model)
    Current view-model uses classic OnGUI
    Input commands (/login username password)
    Command menu (Button list)
    Extendable : write your own view-model


********* Design

ConsoleGUI  - The view script. Responsible for taking the properties found in the ConsoleContext and rendering them via ONGUI.
ConsoleSetup - Monobehaviour responsible for applying optional settings.
ConsoleModel - The centerpiece. Responsible for a non rendering logic.


******** Use
ConsoleContext has a public static accessor.

// write text
ConsoleContext.Instance.Log(message, type);

// write error
ConsoleContext.Instance.LogError(message);

// register command
ConsoleContext.Instance.RegisterCommand(key, addToMenu, delegate);

// invoke command
ConsoleContext.Instance.Submit(commandArgs[]);

// clear
ConsoleContext.Instance.Clear();




