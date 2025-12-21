using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs;

    [Header("Spawn Points")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("Rotación visual")]
    public float rotationY_P1;
    public float rotationY_P2;

    public float selectionScale;

    private int p1Index = 0;
    private int p2Index = 0;

    private GameObject p1Model;
    private GameObject p2Model;

    private bool p1Locked = false;
    private bool p2Locked = false;

    public string selectedCharacterP1;
    public string selectedCharacterP2;

    public GameObject startButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnCharacters();
        startButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!p1Locked)
        {
            if (Input.GetKeyDown(KeyCode.A)) ChangeCharacter(1, -1);
            if (Input.GetKeyDown(KeyCode.D)) ChangeCharacter(1, 1);
            if (Input.GetKeyDown(KeyCode.W)) LockCharacter(1);
        }

        if (!p2Locked)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) ChangeCharacter(2, -1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) ChangeCharacter(2, 1);
            if (Input.GetKeyDown(KeyCode.UpArrow)) LockCharacter(2);
        }

        if (p1Locked && p2Locked)
        {
            startButton.SetActive(true);
        }
    }

    void ChangeCharacter(int player, int direction)
    {
        if (player == 1)
        {
            Destroy(p1Model);
            p1Index = (p1Index + direction + characterPrefabs.Length) % characterPrefabs.Length;
            p1Model = Instantiate(characterPrefabs[p1Index], player1Spawn.position, Quaternion.identity);
            p1Model.transform.localScale = Vector3.one * selectionScale;
            SetSelectionRotation(p1Model, rotationY_P1);
        }
        else if (player == 2)
        {
            Destroy(p2Model);
            p2Index = (p2Index + direction + characterPrefabs.Length) % characterPrefabs.Length;
            p2Model = Instantiate(characterPrefabs[p2Index], player2Spawn.position, Quaternion.identity);
            p2Model.transform.localScale = Vector3.one * selectionScale;
            SetSelectionRotation(p2Model, rotationY_P2);
        }
    }

    void SpawnCharacters()
    {
        p1Model = Instantiate(characterPrefabs[p1Index], player1Spawn.position, Quaternion.identity);
        p2Model = Instantiate(characterPrefabs[p2Index], player2Spawn.position, Quaternion.identity);

        p1Model.transform.localScale = Vector3.one * selectionScale;
        p2Model.transform.localScale = Vector3.one * selectionScale;

        SetSelectionRotation(p1Model, rotationY_P1);
        SetSelectionRotation(p2Model, rotationY_P2);
    }

    void LockCharacter(int player)
    {
        if (player == 1)
        {
            p1Locked = true;
            selectedCharacterP1 = characterPrefabs[p1Index].name;
        }
        else if (player == 2)
        {
            p2Locked = true;
            selectedCharacterP2 = characterPrefabs[p2Index].name;
        }
    }

    public void StartMatch()
    {
        SceneManager.LoadScene("FirstScene");
    }

    void SetSelectionRotation(GameObject model, float yRotation)
    {
        model.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
