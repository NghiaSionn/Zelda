using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectedButtonPanel : MonoBehaviour
{
    [Header("Nút Bấm")]
    public Button invenButton;
    public Button profileButton;
    public Button skillButton;

    [Header("Panel UI")]
    public GameObject selecPanel;
    public GameObject invenPanel;
    public GameObject profilePanel;
    public GameObject skillPanel;

    [Header("Animator")]
    public Animator ivenAnimator;
    public Animator profileAnimator;
    public Animator skillAnimator;

    private bool isInvenOpen = false;
    private bool isProfileOpen = false;
    private bool isSkillOpen = false;

    // Start is called before the first frame update
    void Start()
    {       
        UnActive();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!isInvenOpen)
            {
                
                StartCoroutine(OpenInvenPanel());
            }
            else
            {

                StartCoroutine(CloseInvenPanel());
            }

        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isProfileOpen)
            {

                StartCoroutine(OpenProfilePanel());
            }
            else
            {

                StartCoroutine(CloseProfilePanel());
            }

        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (!isSkillOpen)
            {
                
                StartCoroutine(OpenSkillPanel());
            }
            else
            {
                
                StartCoroutine(CloseSkillPanel());
            }

        }
    }

    private IEnumerator CloseSkillPanel()
    {
        isSkillOpen = false;
        profileAnimator.SetTrigger("Disabled");
        ivenAnimator.SetTrigger("Disabled");
        skillAnimator.SetTrigger("Disabled");

        yield return null;

        skillPanel.SetActive(false);
        selecPanel.SetActive(false);
        invenPanel.SetActive(false);
        profilePanel.SetActive(false);
    }

    private IEnumerator OpenSkillPanel()
    {
        isSkillOpen = true;
        skillPanel.SetActive(true);
        skillAnimator.SetTrigger("Selected");

        yield return null;

        selecPanel.SetActive(true);
        invenPanel.SetActive(false);
        profilePanel.SetActive(false);
    }

    private IEnumerator CloseProfilePanel()
    {
        isProfileOpen = false;
        profileAnimator.SetTrigger("Disabled");
        ivenAnimator.SetTrigger("Disabled");
        skillAnimator.SetTrigger("Disabled");

        yield return null;

        profilePanel.SetActive(false);
        selecPanel.SetActive(false);
        invenPanel.SetActive(false);
        skillPanel.SetActive(false);
    }

    private IEnumerator OpenProfilePanel()
    {
        isProfileOpen = true;
        profilePanel.SetActive(true);
        profileAnimator.SetTrigger("Selected");

        yield return null;

        selecPanel.SetActive(true);
        invenPanel.SetActive(false);
        skillPanel.SetActive(false);
    }

    private IEnumerator CloseInvenPanel()
    {
        isInvenOpen = false;
        ivenAnimator.SetTrigger("Disabled");
        profileAnimator.SetTrigger("Disabled");
        skillAnimator.SetTrigger("Disabled");

        yield return null;

        selecPanel.SetActive(false);
        profilePanel.SetActive(false);
        skillPanel.SetActive(false);
        
    }

    private IEnumerator OpenInvenPanel()
    {
        isInvenOpen = true;      
        selecPanel.SetActive(true);
        ivenAnimator.SetTrigger("Selected");

        yield return null;

        profilePanel.SetActive(false);
        skillPanel.SetActive(false);
        
    }

    private void UnActive()
    {
        selecPanel.SetActive(false);
        profilePanel.SetActive(false);
        skillPanel.SetActive(false);
        invenPanel.SetActive(false);
    }

    private void Active()
    {
        invenPanel.SetActive(true);
        profilePanel.SetActive(true);
        skillPanel.SetActive(true);
        selecPanel.SetActive(true);
    }
}
