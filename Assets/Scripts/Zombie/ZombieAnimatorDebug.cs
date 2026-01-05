using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script untuk debug animator zombie
/// Attach script ini ke zombie untuk melihat status animator secara real-time
/// </summary>
public class ZombieAnimatorDebug : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool showDebugGUI = true;
    
    [Header("Animator Status (Read Only)")]
    [SerializeField] private bool hasAnimator;
    [SerializeField] private bool hasController;
    [SerializeField] private bool hasIsAttackParameter;
    [SerializeField] private bool hasIsRunningParameter;
    [SerializeField] private string currentStateName;
    [SerializeField] private float currentStateTime;
    [SerializeField] private int currentStateHash;
    
    [Header("Real-time State Info")]
    [SerializeField] private string layerName;
    [SerializeField] private bool isInTransition;
    [SerializeField] private int parameterCount;
    
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        CheckAnimatorSetup();
    }
    
    void Update()
    {
        UpdateDebugInfo();
    }
    
    private void CheckAnimatorSetup()
    {
        Debug.Log("========== ZOMBIE ANIMATOR DEBUG CHECK ==========");
        
        if (animator == null)
        {
            Debug.LogError($"??? [{gameObject.name}] Animator component TIDAK DITEMUKAN!");
            hasAnimator = false;
            return;
        }
        
        hasAnimator = true;
        Debug.Log($"? [{gameObject.name}] Animator found!");
        
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError($"??? [{gameObject.name}] Animator Controller TIDAK DI-ASSIGN!");
            hasController = false;
            return;
        }
        
        hasController = true;
        Debug.Log($"? [{gameObject.name}] Animator Controller: {animator.runtimeAnimatorController.name}");
        
        // Cek parameter isAttack
        bool foundIsAttack = false;
        bool foundIsRunning = false;
        
        parameterCount = animator.parameters.Length;
        Debug.Log($"Total parameters: {parameterCount}");
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            Debug.Log($"   - {param.name} ({param.type})");
            
            if (param.name == "isAttack")
            {
                foundIsAttack = true;
                if (param.type != AnimatorControllerParameterType.Trigger)
                {
                    Debug.LogError($"? Parameter 'isAttack' is {param.type}, should be TRIGGER!");
                }
                else
                {
                    Debug.Log($"? Parameter 'isAttack' is TRIGGER - CORRECT!");
                }
            }
            
            if (param.name == "isRunning")
            {
                foundIsRunning = true;
                if (param.type != AnimatorControllerParameterType.Bool)
                {
                    Debug.LogWarning($"?? Parameter 'isRunning' is {param.type}, should be BOOL");
                }
                else
                {
                    Debug.Log($"? Parameter 'isRunning' is BOOL - CORRECT!");
                }
            }
        }
        
        hasIsAttackParameter = foundIsAttack;
        hasIsRunningParameter = foundIsRunning;
        
        if (!foundIsAttack)
        {
            Debug.LogError($"??? [{gameObject.name}] Parameter 'isAttack' NOT FOUND!");
            Debug.LogError("FIX: Open Animator window ? Parameters tab ? Add 'isAttack' as TRIGGER");
        }
        
        if (!foundIsRunning)
        {
            Debug.LogError($"??? [{gameObject.name}] Parameter 'isRunning' NOT FOUND!");
            Debug.LogError("FIX: Open Animator window ? Parameters tab ? Add 'isRunning' as BOOL");
        }
        
        // Check layers
        Debug.Log($"Animator has {animator.layerCount} layer(s)");
        for (int i = 0; i < animator.layerCount; i++)
        {
            Debug.Log($"   Layer {i}: {animator.GetLayerName(i)}");
        }
        
        Debug.Log("================================================");
    }
    
    private void UpdateDebugInfo()
    {
        if (animator == null || !animator.isActiveAndEnabled) return;
        
        if (animator.runtimeAnimatorController != null)
        {
            // Get current state info
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            currentStateHash = stateInfo.fullPathHash;
            currentStateTime = stateInfo.normalizedTime;
            isInTransition = animator.IsInTransition(0);
            layerName = animator.GetLayerName(0);
            
            // Get current state name
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                currentStateName = clipInfo[0].clip.name;
            }
            else
            {
                currentStateName = "Unknown/No Clip";
            }
        }
    }
    
    // Manual trigger untuk testing
    [ContextMenu("Test Attack Animation")]
    public void TestAttackAnimation()
    {
        Debug.Log("========== MANUAL TEST: ATTACK ANIMATION ==========");
        
        if (animator == null)
        {
            Debug.LogError("? Animator is NULL!");
            return;
        }
        
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("? Animator Controller is NULL!");
            return;
        }
        
        Debug.Log($"Current State: {currentStateName}");
        Debug.Log($"State Time: {currentStateTime}");
        Debug.Log($"In Transition: {isInTransition}");
        
        Debug.Log("Triggering 'isAttack'...");
        animator.SetTrigger("isAttack");
        Debug.Log("? SetTrigger('isAttack') called!");
        
        // Log state after trigger (next frame will show actual transition)
        Invoke("LogStateAfterTrigger", 0.1f);
        
        Debug.Log("===================================================");
    }
    
    private void LogStateAfterTrigger()
    {
        Debug.Log("State 0.1s after trigger:");
        Debug.Log($"  Current State: {currentStateName}");
        Debug.Log($"  In Transition: {isInTransition}");
        
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"  State Hash: {info.fullPathHash}");
        Debug.Log($"  State Time: {info.normalizedTime}");
    }
    
    [ContextMenu("Test Run Animation ON")]
    public void TestRunAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }
        
        Debug.Log($"[{gameObject.name}] Setting 'isRunning' to TRUE...");
        animator.SetBool("isRunning", true);
        Debug.Log($"Current State: {currentStateName}");
    }
    
    [ContextMenu("Test Run Animation OFF")]
    public void StopRunAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }
        
        Debug.Log($"[{gameObject.name}] Setting 'isRunning' to FALSE...");
        animator.SetBool("isRunning", false);
        Debug.Log($"Current State: {currentStateName}");
    }
    
    [ContextMenu("Print All Transitions")]
    public void PrintAllTransitions()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator or Controller is null!");
            return;
        }
        
        Debug.Log("========== ANIMATOR TRANSITIONS ==========");
        
#if UNITY_EDITOR
        UnityEditor.Animations.AnimatorController controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        if (controller != null)
        {
            foreach (var layer in controller.layers)
            {
                Debug.Log($"Layer: {layer.name}");
                foreach (var state in layer.stateMachine.states)
                {
                    Debug.Log($"  State: {state.state.name}");
                    foreach (var transition in state.state.transitions)
                    {
                        Debug.Log($"    ? Destination: {transition.destinationState?.name ?? "Exit"}");
                        Debug.Log($"      Has Exit Time: {transition.hasExitTime}");
                        Debug.Log($"      Exit Time: {transition.exitTime}");
                        Debug.Log($"      Duration: {transition.duration}");
                        Debug.Log($"      Conditions: {transition.conditions.Length}");
                        foreach (var condition in transition.conditions)
                        {
                            Debug.Log($"        - {condition.parameter} {condition.mode} {condition.threshold}");
                        }
                    }
                }
                
                // Check Any State transitions
                Debug.Log($"  Any State Transitions:");
                foreach (var transition in layer.stateMachine.anyStateTransitions)
                {
                    Debug.Log($"    ? Destination: {transition.destinationState?.name}");
                    Debug.Log($"      Can Transition To Self: {transition.canTransitionToSelf}");
                    Debug.Log($"      Has Exit Time: {transition.hasExitTime}");
                    Debug.Log($"      Duration: {transition.duration}");
                    Debug.Log($"      Conditions: {transition.conditions.Length}");
                    foreach (var condition in transition.conditions)
                    {
                        Debug.Log($"        - {condition.parameter} {condition.mode}");
                    }
                }
            }
        }
#endif
        
        Debug.Log("==========================================");
    }
    
    private void OnGUI()
    {
        if (!showDebugGUI || !Application.isPlaying) return;
        
        float startY = 10 + (GetInstanceID() % 3) * 210; // Offset untuk multiple zombies
        
        GUILayout.BeginArea(new Rect(10, startY, 350, 200));
        
        GUI.backgroundColor = Color.black;
        GUILayout.Box($"Zombie Animator Debug: {gameObject.name}");
        GUI.backgroundColor = Color.white;
        
        GUILayout.Label($"Has Animator: {hasAnimator}");
        GUILayout.Label($"Has Controller: {hasController}");
        GUILayout.Label($"Has 'isAttack': {hasIsAttackParameter}");
        GUILayout.Label($"Has 'isRunning': {hasIsRunningParameter}");
        GUILayout.Label($"Current State: {currentStateName}");
        GUILayout.Label($"State Time: {currentStateTime:F2}");
        GUILayout.Label($"In Transition: {isInTransition}");
        
        if (GUILayout.Button("FORCE ATTACK"))
        {
            TestAttackAnimation();
        }
        
        if (GUILayout.Button("Print Transitions"))
        {
            PrintAllTransitions();
        }
        
        GUILayout.EndArea();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ZombieAnimatorDebug))]
public class ZombieAnimatorDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ZombieAnimatorDebug script = (ZombieAnimatorDebug)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Manual Testing", EditorStyles.boldLabel);
        
        if (GUILayout.Button("?? TEST ATTACK ANIMATION", GUILayout.Height(30)))
        {
            script.TestAttackAnimation();
        }
        
        if (GUILayout.Button("Test Run Animation (ON)"))
        {
            script.TestRunAnimation();
        }
        
        if (GUILayout.Button("Test Run Animation (OFF)"))
        {
            script.StopRunAnimation();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("?? Print All Transitions Info"))
        {
            script.PrintAllTransitions();
        }
    }
}
#endif
