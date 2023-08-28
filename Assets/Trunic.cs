using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class Trunic : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
    public MeshRenderer Display;
    public Texture[] trunicButtonTextTextures;
    public Texture[] trunicDisplayTextTextures;
    public MeshRenderer[] Buttons;
    public MeshRenderer[] StageLEDS;
    public Texture[] LEDLights;
    public KMSelectable[] ButtonSelectables;


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
            for(int i = 0; i<_buttonTexts.Length; i++) {
                _buttonTexts[i].mainTexture = value[i];
                Buttons[i].transform.localScale = new Vector3(value[i].width / 1134f * 0.0001261897f, Display.transform.localScale.y, Display.transform.localScale.z);
            }
        }
    }
    private int[] buttonTextsNumber;
    private readonly int[] eyeLookupTable = new int[] { 3, 2, 6, 2, 6, 3, 5, 4, 6, 3, 6, 4, 4, 5, 5, 6, 4, 6, 4, 4, 1, 6, 5, 4, 3, 6, 2, 6 };
    private readonly int[,] listLookupTable = new int[,] {
        { 19, 20, 8, 14, 22, 23, 12, 21, 4, 26, 15, 9, 5, 0 },
        { 17, 25, 7, 20, 16, 2, 11, 8, 6, 18, 15, 13, 14, 1 },
        { 4, 22, 17, 23, 21, 20, 6, 13, 1, 7, 14, 24, 26, 2 },
        { 15, 1, 26, 20, 19, 18, 17, 14, 25, 0, 6, 20, 12, 3 },
        { 6, 0, 25, 20, 14, 23, 13, 10, 3, 16, 17, 24, 8, 4 },
        { 16, 4, 21, 15, 9, 11, 8, 13, 19, 23, 25, 10, 12, 5 },
        { 23, 26, 2, 19, 3, 21, 24, 18, 17, 25, 8, 27, 7, 6 },
        { 4, 17, 9, 23, 22, 11, 1, 3, 24, 5, 20, 15, 13, 7 },
        { 1, 13, 10, 26, 23, 19, 16, 4, 12, 3, 11, 18, 5, 8 },
        { 5, 20, 10, 23, 16, 18, 15, 24, 13, 1, 21, 7, 12, 9 },
        { 12, 14, 11, 2, 23, 1, 22, 4, 26, 24, 13, 3, 17, 10 },
        { 17, 12, 7, 22, 9, 3, 20, 21, 13, 8, 24, 14, 19, 11 },
        { 24, 18, 0, 25, 11, 4, 7, 3, 21, 23, 8, 15, 17, 12 },
        { 3, 1, 17, 7, 11, 23, 14, 4, 21, 16, 22, 6, 18, 13 },
        { 21, 23, 10, 5, 25, 2, 4, 6, 9, 16, 17, 11, 19, 14 },
        { 19, 18, 20, 23, 4, 5, 13, 9, 7, 3, 10, 1, 2, 15 },
        { 6, 10, 0, 11, 21, 17, 1, 2, 24, 13, 19, 4, 15, 16 },
        { 20, 8, 3, 19, 7, 4, 11, 24, 22, 0, 14, 18, 26, 17 },
        { 10, 0, 26, 2, 25, 13, 1, 6, 8, 17, 14, 11, 12, 18 },
        { 2, 8, 15, 11, 10, 23, 16, 7, 17, 0, 13, 24, 21, 19 },
        { 1, 8, 7, 16, 17, 2, 12, 19, 11, 21, 13, 26, 5, 20 },
        { 3, 6, 13, 10, 2, 22, 11, 26, 17, 24, 1, 9, 19, 21 },
        { 19, 25, 8, 18, 23, 15, 26, 0, 22, 3, 9, 2, 12, 22 },
        { 15, 26, 13, 20, 25, 2, 4, 7, 21, 10, 22, 12, 17, 23 },
        { 3, 20, 11, 1, 8, 9, 4, 21, 16, 22, 23, 17, 26, 24 },
        { 14, 10, 26, 4, 13, 5, 19, 2, 24, 15, 8, 0, 9, 25 },
        { 7, 25, 24, 18, 13, 21, 5, 6, 12, 10, 2, 17, 20, 26 },
        { 12, 24, 1, 4, 21, 15, 16, 0, 3, 8, 22, 7, 14, 27 },
    };
    private int Key;
    private int SolButton;
    private Animation ButtonAnimation;
    private int Stage = 1;

    void Awake () { //Shit that happens before Start
        ModuleName = Module.ModuleDisplayName;
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;

        _displayText = Display.material;
        _buttonTexts = new Material[Buttons.Length];
        buttonTextsNumber = new int[Buttons.Length];
        for(int i=0; i<Buttons.Length; i++) {
            _buttonTexts[i] = Buttons[i].material;
        }
        for(int i = 0; i<ButtonSelectables.Length; i++) {
            int dummy = i;
            ButtonSelectables[dummy].OnInteract += delegate () { buttonPress(dummy); return false; };
        }

        ButtonAnimation = GetComponent<Animation>();
        //button.OnInteract += delegate () { buttonPress(); return false; };
    } 

    void buttonPress(int Number) {
        KMSelectable self = ButtonSelectables[Number];
        self.AddInteractionPunch();
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, self.transform);
        if(ModuleSolved) {
            return;
        }
        Log($"Pressing {(Number+1).ToString()}.");
        if(Number == SolButton) {
            if(Stage == 3) {
                Solve();
            } else{
                StartCoroutine(StageUp());
            }
        } else {
            StartCoroutine(Strike());
        }
    }

    void Start () { //Shit
        displayNumber = Rnd.Range(0, trunicDisplayTextTextures.Length);
        DisplayText = trunicDisplayTextTextures[displayNumber];
        Match m = Regex.Match(trunicDisplayTextTextures[displayNumber].name, @"(\w+)W");
        Log($"The Displayy says {m.Groups[1].Value}.");
        Texture[] temp = new Texture[ButtonTexts.Length];
        Log($"The buttons for Stage {Stage.ToString()} are labeled:");
        for(int i=0; i<buttonTextsNumber.Length; i++) {
            bool NewLabel = false;
            while(!NewLabel) {
                NewLabel = true;
                buttonTextsNumber[i] = Rnd.Range(0, trunicButtonTextTextures.Length);
                for(int f = 0; f < i; f++) {
                    if(buttonTextsNumber[f] == buttonTextsNumber[i]) {
                        NewLabel = false;
                    }
                }
            }
            temp[i] = trunicButtonTextTextures[buttonTextsNumber[i]];
            Log(trunicButtonTextTextures[buttonTextsNumber[i]].name);
        }
        ButtonTexts = temp;
        Key = buttonTextsNumber[eyeLookupTable[displayNumber]-1];
        SolButton = -1;
        for(int i = 0; SolButton == -1; i++) {
            SolButton = Array.IndexOf(buttonTextsNumber, listLookupTable[Key, i]);
            if(i == 14 && SolButton == -1) {
                SolButton = 0;
            }
        }
        Log($"Correct button is {(SolButton+1).ToString()}. ");
    }

    void Activate() { //Shit that should happen when the bomb arrives (factory)/Lights turn on

    }

    void Update () { //Shit that happens at any point after initialization

    }

    void Solve () { //Call this method when you want the module to solve
        foreach(MeshRenderer i in StageLEDS) {
            i.material.mainTexture = LEDLights[1];
        }
        Module.HandlePass();
        Log("Correct! Module Solved.");
        ModuleSolved = true;
    }

    IEnumerator Strike () { //Call this method when you want the module to strike
        Module.HandleStrike();
        Log("Incorrect! Strike Issued.");
        ButtonAnimation.Play("Stage Up");
        foreach(MeshRenderer i in StageLEDS) {
            i.material.mainTexture = LEDLights[0];
        }
        Stage = 1;
        Display.enabled = false;
        yield return new WaitForSeconds(1);
        Start();
        yield return new WaitForSeconds(1);
        Display.enabled = true;
    }

    IEnumerator StageUp() {
        Log($"Stage {Stage.ToString()} Correct.");
        ButtonAnimation.Play("Stage Up");
        StageLEDS[Stage - 1].material.mainTexture = LEDLights[1];
        Stage++;
        Display.enabled = false;
        yield return new WaitForSeconds(1);
        Start();
        yield return new WaitForSeconds(1);
        Display.enabled = true;
    }

    void Log (string message) {
        Debug.Log($"[{ModuleName} #{ModuleId}] {message}");
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press 3 [press the third button in english reading order.]";
#pragma warning restore 414

    // Twitch Plays (TP) documentation: https://github.com/samfundev/KtaneTwitchPlays/wiki/External-Mod-Module-Support

    KMSelectable[] ProcessTwitchCommand (string Command) {
        string COMMAND = Command.ToUpper().Trim();
        Match m = Regex.Match(COMMAND, "(PRESS [1-6])$");
        if(m.Success) {
            return new KMSelectable[] { ButtonSelectables[int.Parse(m.Value.Last().ToString())-1] };
        }
        m = Regex.Match(COMMAND, "([A-Z][A-Z]|[A-Z][A-Z][A-Z]|[A-Z][A-Z][A-Z][A-Z]|[A-Z][A-Z][A-Z][A-Z][A-Z])$");
        if(m.Success) {
            for(int i = 0; i <6; i++) {
                if(ButtonTexts[i].name.ToUpper() == m.Value) {
                    return new KMSelectable[] { ButtonSelectables[i] };
                }
            }
        }
        return null;
    }

    void TwitchHandleForcedSolve () {
        Solve();
        return;
    }
}
