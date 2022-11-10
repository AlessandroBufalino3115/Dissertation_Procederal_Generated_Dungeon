using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiBaseState 
{

    public abstract void onStart(StateUIManager currentMenu);

    public abstract void onUpdate(StateUIManager currentMenu);

    public abstract void onExit(StateUIManager currentMenu);

    public abstract void onGUI(StateUIManager currentMenu);
}
