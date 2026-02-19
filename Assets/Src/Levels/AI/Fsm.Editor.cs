#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Levels.AI
{
    [CustomEditor(typeof(Fsm))]
    public class FsmEditor : Editor
    {
        private const string Header = @"
!theme plain
hide empty description

<style>
    legend {
        LineColor White
    }
    start {
        BackGroundColor Black
    }
</style>

legend top left
    |= Color |= Form |= Description |
    | <#green> | '⇢' (dashed arrow) | This state can transition \n into the pointed |
    | <#orange> | 'ⓧ→' (cross then arrow) | Pointed can override \n the state when ready |
    | <#red> | 'ⓧ→' (cross then arrow) | This state will be replaced \n by the pointed when ends |
endlegend
";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("Copy PlantUML"))
            {
                var fsm = (Fsm)target;

                var diagram = GeneratePlantUml(fsm);

                EditorGUIUtility.systemCopyBuffer = diagram;

                Debug.Log("FSM diagram copied to clipboard.");
            }
        }

        private string GeneratePlantUml(Fsm fsm)
        {
            var sb = new StringBuilder();

            sb.AppendLine("@startuml");
            sb.AppendLine(Header);

            var states = fsm.States;

            var enterState = states.Count > 0 ? states[0].GetType().Name : "(None)";
            sb.AppendLine($"[*] --> {enterState}\n");

            foreach (var state in states)
            {
                var stateName = state.GetType().Name;

                foreach (var next in state.NextStates)
                {
                    sb.AppendLine($"{stateName} -[#green,dashed]-> {next.GetType().Name}");
                }

                foreach (var ov in state.OverridesStates)
                {
                    sb.AppendLine($"{stateName} <-[#orange]-x {ov.GetType().Name}");
                }

                var fallback = state.Fallback;
                if (fallback is not null)
                {
                    sb.AppendLine($"{stateName} x-[#red]-> {fallback.GetType().Name}");
                }

                sb.AppendLine();
            }

            sb.AppendLine("@enduml");

            return sb.ToString();
        }
    }

    public partial class FsmState
    {
        public IReadOnlyList<FsmState> NextStates => nextStates;
        public IReadOnlyList<FsmState> OverridesStates => overridesStates;
        public FsmState Fallback => fallback;
    }
}
#endif