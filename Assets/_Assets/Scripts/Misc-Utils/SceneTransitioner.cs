using System.Collections;
using System.Collections.Generic;
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

    public void LoadScene(int _newSceneIndex, Vector3 _spawnPosition, Vector3 _followerDirection)
    {
        placePartyMembers = true;
        spawnPosition = _spawnPosition;
        followerDirection = _followerDirection;

        SceneManager.LoadScene(_newSceneIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
