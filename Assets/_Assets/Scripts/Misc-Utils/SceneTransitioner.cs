using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : Singleton<SceneTransitioner>
{
    [Header("Debug")]
    [SerializeField] private int sceneToLoadIndex = 1;

    private bool placePartyMembers = false;
    private Vector3 spawnPosition;
    private Vector3 followerDirection;
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

    public IEnumerator StartLoadScene(int _newSceneIndex, Vector3 _spawnPosition, Vector3 _followerDirection)
    {
        placePartyMembers = true;
        spawnPosition = _spawnPosition;
        followerDirection = _followerDirection;

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
        var scene = SceneManager.LoadSceneAsync(_newSceneIndex);
        scene.allowSceneActivation = false;

        await Task.Delay(250);

        do
        {
            Debug.Log("Load progress: " + scene.progress);
            await Task.Delay(1);
        }
        while (scene.progress < 0.9f);

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
            for (int i = 0; i < PartyManager.Instance.PartyMembers.Count; i++)
            {
                Transform partyMember = PartyManager.Instance.PartyMembers[i].transform.parent;
                partyMember.position = spawnPosition + followerDirection * i * partyMemberSpacing;
            }
            placePartyMembers = false;
        }
    }
}
