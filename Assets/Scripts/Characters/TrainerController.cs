using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    [SerializeField] GameObject exclamtion;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject fov;

    bool battleLost = false;

    Characters characters;

    private void Awake()
    {
        characters = GetComponent<Characters>();
    }

    private void Start()
    {
        SetFovRotation(characters.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        //Show Exclamtion
        exclamtion.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamtion.SetActive(false);

        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return characters.Move(moveVec);

        //Show Dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            //Debug.Log("Trainer Battle!");
            GameController.Instance.StartTrainerBattle(this);
        }));

    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public void Interact(Transform initiator)
    {
        characters.LookTowards(initiator.position);

        if (!battleLost)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                GameController.Instance.StartTrainerBattle(this);
            }));
        }
        else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle));
        }
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }
}
