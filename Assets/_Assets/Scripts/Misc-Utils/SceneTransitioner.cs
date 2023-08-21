using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : Singleton<SceneTransitioner>
{
    [Header("Debug")]
    [SerializeField] private int sceneToLoadIndex = 1;

    private bool placePartyMembers = false;
    private string targetTriggerID;
    private const float partyMemberSpacing = 0.5f;

    private AnimatorController_2DTopDown playerAnimController;
    private AnimatorController_2DTopDown.MoveStateEnum startingState;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(sceneToLoadIndex);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator StartLoadScene(int _newSceneIndex, string _targetID)
    {
        PauseController.Instance.AddPauser(gameObject);

        placePartyMembers = true;
        targetTriggerID = _targetID;

        ScreenTransitionUI.Instance.DarkAnim.SetBool("IsDark", true);

        playerAnimController = PartyManager.Instance.GetFirstAlivePlayer().transform.parent.GetComponent<AnimatorController_2DTopDown>();
        startingState = playerAnimController.State;
        ScreenTransitionUI.Instance.PlayerAnim.runtimeAnimatorController = playerAnimController.Anim.runtimeAnimatorController;
        ScreenTransitionUI.Instance.PlayerAnim.Play(startingState.ToString(), 0, playerAnimController.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

        yield return new WaitForSeconds(0.25f);

        LoadScene(_newSceneIndex);
    }

    private async void LoadScene(int _newSceneIndex)
    {
        float startTime = Time.time;

        var scene = SceneManager.LoadSceneAsync(_newSceneIndex);
        scene.allowSceneActivation = false;

        const int msToDelay = 250;

        do
        {
            Debug.Log("Load progress: " + scene.progress);
            await Task.Delay(1);
        }
        while (scene.progress < 0.9f);

        int msTaken = Mathf.RoundToInt((Time.time - startTime) * 1000);
        if (msTaken < msToDelay)
        {
            //Loading screen will show up for at least 250ms
            await Task.Delay(msToDelay - msTaken);
        }

        scene.allowSceneActivation = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ScreenTransitionUI.Instance.DarkAnim.SetBool("IsDark", false);

        if (playerAnimController != null)
        {
            if (playerAnimController.State == startingState)
            {
                //Same animation still playing, make player's anim match the transition UI
                playerAnimController.Anim.Play(startingState.ToString(), 0, ScreenTransitionUI.Instance.PlayerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
        }

        if (placePartyMembers)
        {
            SceneTrigger targTrigger = SceneTriggerHolder.Instance.GetTriggerWithID(targetTriggerID);
            Vector3 spawnPosition = targTrigger.SpawnPosition.position;
            Vector3 followerDirection = targTrigger.SpawnPosition.up;

            for (int i = 0; i < PartyManager.Instance.PartyMembers.Count; i++)
            {
                Transform partyMember = PartyManager.Instance.PartyMembers[i].transform.parent;
                partyMember.position = spawnPosition + i * partyMemberSpacing * followerDirection;
            }
            placePartyMembers = false;
        }

        PauseController.Instance.RemovePauser(gameObject);
    }
}
