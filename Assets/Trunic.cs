    using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

//HI :3 gl with modding


     // this template here need to be the EXACT same thing as yo module type
public class Trunic : KtaneModule {
     // might aswell name the script file the same thing

    // Modding Tutorial by Deaf: https://www.youtube.com/watch?v=YobuGSBl3i0

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
    public MeshRenderer Display;
    public Texture[] trunicButtonTextTextures;
    public Texture[] trunicDisplayTextTextures;
    public MeshRenderer[] Buttons;
    public MeshRenderer[] StageLEDS;
    public Texture[] LEDLights;

    string ModuleName;
    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;
    private Material _displayText;
    private Texture DisplayText {
        get { return _displayText.mainTexture; } 
        set {
            _displayText.mainTexture = value;
            Display.transform.localScale = new Vector3(value.width/1134f*0.0001261897f, Display.transform.localScale.y, Display.transform.localScale.z);
        }
    }
    private int displayNumber;
    private Material[] _buttonTexts;
    private Texture[] ButtonTexts {
        get {
            Texture[] temp = new Texture[_buttonTexts.Length];
            for(int i = 0; i<_buttonTexts.Length; i++) {
                temp[i] = _buttonTexts[i].mainTexture;
            }
            return temp;
        }
        set {
            Texture[] temp = new Texture[_buttonTexts.Length];
            for(int i = 0; i<_buttonTexts.Length; i++) {
                temp[i] = value[i].
            }
        }
    }
    //3 0.0003351862
    //2 0.0002381024
    //1 0.0001370526

    new void Awake () { //Shit that happens before Start
        ModuleName = Module.ModuleDisplayName;
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;
        _displayText = Display.material;
        _buttonTexts = new Material[Buttons.Length];
        for(int i=0; i<Buttons.Length; i++) {
            _buttonTexts[i] = Buttons[i].material;
        }
        displayNumber = Rnd.Range(0, trunicDisplayTextTextures.Length);
        DisplayText = trunicDisplayTextTextures[displayNumber];
        Log(trunicDisplayTextTextures[displayNumber].name);
        /*
         * How to make buttons work:
         * 
        foreach (KMSelectable object in keypad) {
            object.OnInteract += delegate () { keypadPress(object); return false; };
        }
        */

        //button.OnInteract += delegate () { buttonPress(); return false; };

        //keypadPress() and buttonPress() you have to make yourself and should just be what happens when you press a button. (deaf goes through it probably)
    }

    void Activate () { //Shit that should happen when the bomb arrives (factory)/Lights turn on

    }

    new void Start () { //Shit

    }

    new void Update () { //Shit that happens at any point after initialization

    }

    void Solve () { //Call this method when you want the module to solve
        Module.HandlePass();
        Log("Correct! Module Solved.");
        ModuleSolved = true;
    }

    void Strike () { //Call this method when you want ot module to strike
        Module.HandleStrike();
        Log("Incorrect! Strike Issued.");
    }

    void Log (string message) { //I did the logging for you <3. just do Log("message"); for logging (and please log like OMG just log its so easy esp with this)
        //If this underlined red for you (giving a compiler error), hover it and click on like the show possible fixes text, then click on upgrade this/all projects to c# version 6.
        Debug.Log($"[{ModuleName} #{ModuleId}] {message}");
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

    // Twitch Plays (TP) documentation: https://github.com/samfundev/KtaneTwitchPlays/wiki/External-Mod-Module-Support

    IEnumerator ProcessTwitchCommand (string Command) {
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve () {
        yield return null;
    }
}
