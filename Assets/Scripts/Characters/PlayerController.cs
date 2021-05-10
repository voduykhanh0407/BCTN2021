using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    private Vector2 input;

    private Characters characters;

    private void Awake()
    {
        characters = GetComponent<Characters>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!characters.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                StartCoroutine(characters.Move(input, OnMoveOver));
            }
        }

        characters.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    void Interact()
    {
        var facingDir = new Vector3(characters.Animator.MoveX, characters.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        //CheckForEncounters();
        //CheckIfInTrainersView();
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, characters.OffsetY), 0.2f, GameLayers.i.TriggerableLayer);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if( triggerable != null)
            {
                characters.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    public Characters Characters => characters;
}
