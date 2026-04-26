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
    | <#blue> | '⇢' (dashed arrow) | Potential successor states. When this state finishes,\nthe FSM will transition to the first state in this list that is 'Ready'. |
    | <#orange> | 'ⓧ→' (cross then arrow) | A list of states that THIS state is allowed to interrupt.\nIf this state becomes 'Ready' while one of these is active, it will force a transition to itself. |
    | <#gray> | 'ⓧ→' (cross then arrow) | The safety-net state. If this state finishes and none of the 'Next States' are ready,\nthe FSM will transition here. |
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

            for (var i = 0; i < states.Count - 1; i++)
            {
                sb.AppendLine($"{states[i].GetType().Name} -[hidden]down-> {states[i + 1].GetType().Name}");
            }
            sb.AppendLine();

            var enterState = states.Count > 0 ? states[0].GetType().Name : "(None)";
            sb.AppendLine($"[*] --> {enterState}\n");

            foreach (var state in states)
            {
                var stateName = state.GetType().Name;

                if (state.NextStates.Count == 1)
                {
                    sb.AppendLine($"{stateName} -[#blue,dashed]-> {state.NextStates[0].GetType().Name}");
                }
                else for (var i = 0; i < state.NextStates.Count; i++)
                {
                    var next = state.NextStates[i];
                    sb.AppendLine($"{stateName} -[#blue,dashed]-> {next.GetType().Name} : [{i}]");
                }

                foreach (var ov in state.OverridesStates)
                {
                    sb.AppendLine($"{stateName} <-[#orange]-x {ov.GetType().Name}");
                }

                var fallback = state.Fallback;
                if (fallback is not null)
                {
                    sb.AppendLine($"{stateName} x-[#gray]-> {fallback.GetType().Name}");
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