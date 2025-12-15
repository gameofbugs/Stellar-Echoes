
using UnityEngine;

using UnityEngine.UI;


public class UiAnimations : MonoBehaviour

{

    private Graphic[] graphics;

    private bool initialized = false; // Prevent fade on first scene load


    void Awake()

    {

        graphics = GetComponentsInChildren<Graphic>(true);

        SetAlpha(0f);

    }


    void OnEnable()

    {

        // Only animate after the first activation

        if (initialized)

        {

            PlayFadeIn();

        }

        else

        {

            SetAlpha(1f); // Start fully visible

            initialized = true;

        }

    }


    private void PlayFadeIn()

    {

        SetAlpha(0f);

        LeanTween.value(gameObject, 0f, 1f, 0.5f)

            .setOnUpdate((float val) => SetAlpha(val))

            .setEaseOutCubic();


        transform.localScale = Vector3.one * 0.9f;

        LeanTween.scale(gameObject, Vector3.one, 0.4f).setEaseOutBack();

    }


    public void HidePanel()

    {

        LeanTween.scale(gameObject, Vector3.one * 0.9f, 0.3f).setEaseInBack();

        LeanTween.value(gameObject, 1f, 0f, 0.4f)

            .setOnUpdate((float val) => SetAlpha(val))

            .setEaseInCubic()

            .setOnComplete(() => gameObject.SetActive(false));

    }


    private void SetAlpha(float value)

    {

        foreach (var g in graphics)

        {

            if (g != null)

            {

                Color c = g.color;

                c.a = value;

                g.color = c;

            }

        }

    }

}
