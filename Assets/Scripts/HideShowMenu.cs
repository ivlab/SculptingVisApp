using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShowMenu : MonoBehaviour
{
    [Header("Configuration")]
    /// <summary> Position this menu should move to when hidden. </summary>
    [SerializeField] private Vector2 hidePosition;
    /// <summary> Position this menu should move to when shown. </summary>
    [SerializeField] private Vector2 showPosition;
    /// <summary> Speed at which the menu should move. </summary>
    [SerializeField] private float speed;
    [Header("Dependencies")]
    /// <summary> Arrow icons. Rotate depending on whether the menu is hidden or shown. </summary>
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject leftArrow;
    /// <summary> Used to ensure the arrows rotate in opposite directions when Lerping. </summary>
    private float arrowFudge = 0.0001f;
    /// <summary> Current hidden/shown status of the menu. </summary>
    private bool hidden = true;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Toggles whether the menu is hidden or shown.
    /// </summary>
    public void ToggleHideShow()
    {
        // Toggle hide/show
        hidden = !hidden;
        // Start the corresponding hide/show coroutine
        StopAllCoroutines();  // (prevents simultaneous hide/show of the menu)
        if (hidden)
            StartCoroutine(Hide());
        else
            StartCoroutine(Show());

    }

    /// <summary>
    /// Hides the menu.
    /// </summary>
    private IEnumerator Hide()
    {
        float t = Time.deltaTime * speed;
        while (t < 1.0f)
        {
            rightArrow.transform.localRotation = Quaternion.Lerp(rightArrow.transform.localRotation, Quaternion.Euler(0, 0, 180 + arrowFudge), t * t);
            leftArrow.transform.localRotation = Quaternion.Lerp(leftArrow.transform.localRotation, Quaternion.Euler(0, 0, 180 - arrowFudge), t * t);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, hidePosition, t * t);
            t += Time.deltaTime * speed;
            yield return null;
        }
        rightArrow.transform.localRotation = Quaternion.Euler(0, 0, 180 + arrowFudge);
        leftArrow.transform.localRotation = Quaternion.Euler(0, 0, 180 - arrowFudge);
        rectTransform.anchoredPosition = hidePosition;
    }

    /// <summary>
    /// Shows the menu.
    /// </summary>
    private IEnumerator Show()
    {
        float t = Time.deltaTime * speed;
        while (t < 1.0f)
        {
            rightArrow.transform.localRotation = Quaternion.Lerp(rightArrow.transform.localRotation, Quaternion.Euler(0, 0, 0), t * t);
            leftArrow.transform.localRotation = Quaternion.Lerp(leftArrow.transform.localRotation, Quaternion.Euler(0, 0, 0), t * t);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, showPosition, t * t);
            t += Time.deltaTime * speed;
            yield return null;
        }
        rightArrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
        leftArrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
        rectTransform.anchoredPosition = showPosition;
    }
}
