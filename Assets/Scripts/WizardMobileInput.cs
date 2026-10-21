using UnityEngine;

// Touch input for phones/tablets. No external joystick assets or packages required.
// Left finger controls movement; right-side swipe orbits camera; bottom buttons cast/jump/sprint.
public sealed class WizardMobileInput : MonoBehaviour
{
    private enum TouchRole { None, Move, Camera, Cast, Jump, Sprint }
    private static readonly System.Collections.Generic.Dictionary<int, TouchRole> roles =
        new System.Collections.Generic.Dictionary<int, TouchRole>();
    private static Vector2 move, cameraDelta;
    private static bool cast, jump, sprint;
    private static int lastFrame = -1;

    public static Vector2 Move { get { Poll(); return move; } }
    public static Vector2 CameraDelta { get { Poll(); return cameraDelta; } }
    public static bool CastPressed { get { Poll(); return cast; } }
    public static bool JumpPressed { get { Poll(); return jump; } }
    public static bool Sprint { get { Poll(); return sprint; } }

    private static Vector2 JoystickCenter => new Vector2(Screen.width * .17f, Screen.height * .19f);
    private static float StickRadius => Mathf.Clamp(Screen.width * .105f, 62f, 125f);
    private static Vector2 CastCenter => new Vector2(Screen.width * .85f, Screen.height * .19f);
    private static Vector2 JumpCenter => new Vector2(Screen.width * .70f, Screen.height * .115f);
    private static Vector2 SprintCenter => new Vector2(Screen.width * .70f, Screen.height * .29f);
    private static float ButtonRadius => Mathf.Clamp(Screen.width * .064f, 43f, 84f);

    private static void Poll()
    {
        if (lastFrame == Time.frameCount) return;
        lastFrame = Time.frameCount;
        move = Vector2.zero;
        cameraDelta = Vector2.zero;
        cast = jump = sprint = false;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                TouchRole role;
                if (Vector2.Distance(touch.position, CastCenter) < ButtonRadius * 1.45f) role = TouchRole.Cast;
                else if (Vector2.Distance(touch.position, JumpCenter) < ButtonRadius * 1.45f) role = TouchRole.Jump;
                else if (Vector2.Distance(touch.position, SprintCenter) < ButtonRadius * 1.45f) role = TouchRole.Sprint;
                else if (touch.position.x < Screen.width * .46f) role = TouchRole.Move;
                else role = TouchRole.Camera;
                roles[touch.fingerId] = role;
            }

            TouchRole current;
            if (!roles.TryGetValue(touch.fingerId, out current)) continue;
            bool finished = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
            if (current == TouchRole.Move && !finished)
                move = Vector2.ClampMagnitude((touch.position - JoystickCenter) / StickRadius, 1f);
            if (current == TouchRole.Camera && touch.phase == TouchPhase.Moved)
                cameraDelta += touch.deltaPosition;
            if (current == TouchRole.Cast && touch.phase == TouchPhase.Began) cast = true;
            if (current == TouchRole.Jump && touch.phase == TouchPhase.Began) jump = true;
            if (current == TouchRole.Sprint && !finished) sprint = true;
            if (finished) roles.Remove(touch.fingerId);
        }
    }

    private void OnGUI()
    {
        if (!Application.isMobilePlatform) return;
        GUI.skin.box.fontSize = Mathf.Clamp(Screen.height / 30, 15, 36);
        DrawButton(JoystickCenter, StickRadius * 2f, "MOVE");
        DrawButton(CastCenter, ButtonRadius * 2f, "SPELL");
        DrawButton(JumpCenter, ButtonRadius * 2f, "JUMP");
        DrawButton(SprintCenter, ButtonRadius * 2f, "RUN");
        var caster = GetComponent<WizardSpellCaster>();
        GUI.Box(new Rect(Screen.width * .03f, Screen.height * .03f,
            Screen.width * .33f, Screen.height * .075f),
            caster != null ? "Mana " + Mathf.CeilToInt(caster.mana) + " / 100" : "WizardTome");
    }

    private static void DrawButton(Vector2 center, float diameter, string label)
    {
        GUI.Box(new Rect(center.x - diameter / 2f,
            Screen.height - center.y - diameter / 2f, diameter, diameter), label);
    }
}
