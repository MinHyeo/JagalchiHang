using UnityEngine;
using System;
using System.Threading;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class LoadingUI : UIBase
{
    [SerializeField] private RawImage RawImage_LoadingImg;
    [SerializeField] private string[] _loadingImagePaths;
    [SerializeField] private Image Image_LoadingFill;

    private CancellationTokenSource _cancelToken;
    private readonly float[] _pausePoints = { 0.1f, 0.1f, 0.1f };
    private int _pauseIndex = 0;

    private void OnEnable()
    {
        LoadAndSetLoadingImg();
    }

    private void LoadAndSetLoadingImg()
    {
        if (_loadingImagePaths == null || _loadingImagePaths.Length == 0) 
        {
            Debug.LogWarning($"{name} : 로딩 이미지 후보 경로가 비어있습니다.");
        }
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, _loadingImagePaths.Length); 
            string texturePath = _loadingImagePaths[randomIndex];

            LoadAndSetTexture(RawImage_LoadingImg, texturePath).Forget(); 
        }

        StartLoadingResource(4f).Forget();
    }

    private static async UniTaskVoid LoadAndSetTexture(RawImage rawImage, string texturePath)
    {
        if (ResourceManager.Instance == null) 
        {
            Debug.LogWarning($"LoadingUI : ResourceManager.Instance가 null이라 '{texturePath}'를 로드할 수 없습니다.");
            return;
        }

        Texture2D texture = await ResourceManager.Instance.LoadAsset<Texture2D>(texturePath); // Addressables로 텍스처 로드

        if (texture == null)
        {
            Debug.LogWarning($"LoadingUI : '{texturePath}' 텍스처를 불러오지 못했습니다.");
            return;
        }

        rawImage.texture = texture; 
    }

    private async UniTaskVoid StartLoadingResource(float duration)
    {
        _cancelToken = new CancellationTokenSource();
        float elapsed = 0f;
        _pauseIndex = 0;
        Image_LoadingFill.fillAmount = 0f; 

        while (elapsed < duration) 
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            if (_pauseIndex < _pausePoints.Length && progress >= _pausePoints[_pauseIndex]) 
            {
                float pausePointValue = _pausePoints[_pauseIndex];
                Image_LoadingFill.fillAmount = pausePointValue;

                await UniTask.Delay(TimeSpan.FromSeconds(pausePointValue), cancellationToken: _cancelToken.Token);

                _pauseIndex++;
            }

            Image_LoadingFill.fillAmount = progress;

            await UniTask.Yield(PlayerLoopTiming.Update, _cancelToken.Token); 
        }

        Image_LoadingFill.fillAmount = 1.0f;

        CloseLoadingUI();
    }

    private void CloseLoadingUI()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogWarning($"{name} : UIManager.Instance가 null입니다.");
            return;
        }

        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.LoadingUI); 
    }
}
