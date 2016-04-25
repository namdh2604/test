using UnityEngine;
using System;
using System.Collections;

using Voltage.Witches.Models.Avatar;

public class AvatarView : MonoBehaviour
{
    private AvatarImageDisplay _display;

    [SerializeField]
    private GameObject _loadingIndicator;

    private bool _isZoomed = false;
    private bool _isZooming = false; // whether or not the zoom animation is in progress

    private Vector2 _normalPosition = new Vector2(480.0f, 0.0f);
    private Vector2 _zoomedPosition = new Vector2(526.0f, -1158.0f);

    private readonly Vector3 _normalScale = Vector3.one;
    private readonly Vector3 _zoomedScale = new Vector3(2.5f, 2.5f, 1.0f);

    private const float _zoomTime = 0.5f;

    private RectTransform _rt;

	private void Start()
	{
		GameObject image = AvatarImageDisplay.CreateAvatar(AvatarType.Fullbody);
		image.transform.SetParent(transform, false);
        image.transform.SetAsFirstSibling();
        _display = image.GetComponent<AvatarImageDisplay>();

        _rt = transform as RectTransform;
	}

    public void UpdateTexture()
    {
        // force coordinate reloading -- the file should have changed
        _display.UpdateTexture(true);
    }

    public void ShowLoadingIndicator(bool value)
    {
        _loadingIndicator.SetActive(value);
    }

    public Coroutine ToggleZoom()
    {
        if (!_isZooming)
        {
            _isZooming = true;
            return StartCoroutine(ZoomRoutine());
        }

        return null;
    }

    private IEnumerator ZoomRoutine()
    {
        Vector2 initialPosition;
        Vector2 finalPosition;

        Vector3 initialScale;
        Vector3 finalScale;

        if (_isZoomed)
        {
            initialPosition = _zoomedPosition;
            finalPosition = _normalPosition;

            initialScale = _zoomedScale;
            finalScale = _normalScale;
        }
        else
        {
            initialPosition = _normalPosition;
            finalPosition = _zoomedPosition;

            initialScale = _normalScale;
            finalScale = _zoomedScale;
        }

        object[] translateArgs = new object[]
        {

            "from", initialPosition,
            "to", finalPosition,
            "time", _zoomTime,
            "easetype", iTween.EaseType.linear,
            "onupdate", (Action<object>)(value => _rt.anchoredPosition = (Vector2)value)
        };

        object[] scaleArgs = new object[]
        {
            "from", initialScale,
            "to", finalScale,
            "time", _zoomTime,
            "easetype", iTween.EaseType.linear,
            "onupdate", (Action<object>)(value => _rt.localScale = (Vector3)value)
        };

        iTween.ValueTo(_display.gameObject, iTween.Hash(translateArgs));
        iTween.ValueTo(_display.gameObject, iTween.Hash(scaleArgs));

        yield return new WaitForSeconds(_zoomTime);

        _isZoomed = !_isZoomed;
        _isZooming = false;
    }
}
