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
                ivenAnimator.SetTrigger("Selected");
                OpenInvenPanel();
            }
            else
            {
                ivenAnimator.SetTrigger("Disabled");
                CloseInvenPanel();
            }

        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isProfileOpen)
            {
                profileAnimator.SetTrigger("Selected");
                OpenProfilePanel();
            }
            else
            {
                profileAnimator.SetTrigger("Disabled");
                CloseProfilePanel();
            }

        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (!isSkillOpen)
            {
                skillAnimator.SetTrigger("Selected");
                OpenSkillPanel();
            }
            else
            {
                skillAnimator.SetTrigger("Disabled");
                CloseSkillPanel();
            }

        }
    }

    private void CloseSkillPanel()
    {
        isSkillOpen = false;
        skillPanel.SetActive(false);
        selecPanel.SetActive(false);
        invenPanel.SetActive(false);
        profilePanel.SetActive(false);
    }

    private void OpenSkillPanel()
    {
        isSkillOpen = true;
        skillPanel.SetActive(true);
        selecPanel.SetActive(true);
        invenPanel.SetActive(false);
        profilePanel.SetActive(false);
    }

    private void CloseProfilePanel()
    {
        isProfileOpen = false;
        profilePanel.SetActive(false);
        selecPanel.SetActive(false);
        invenPanel.SetActive(false);
        skillPanel.SetActive(false);
    }

    private void OpenProfilePanel()
    {
        isProfileOpen = true;
        profilePanel.SetActive(true);
        selecPanel.SetActive(true);
        invenPanel.SetActive(false);
        skillPanel.SetActive(false);
    }

    private void CloseInvenPanel()
    {
        isInvenOpen = false;
        selecPanel.SetActive(false);
        profilePanel.SetActive(false);
        skillPanel.SetActive(false);
    }

    private void OpenInvenPanel()
    {
        isInvenOpen = true;      
        selecPanel.SetActive(true);
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
