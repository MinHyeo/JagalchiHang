using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();
        
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    public async UniTask<T> LoadAsset<T>(string address) where T : UnityEngine.Object
    {
        if(_handles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            return handle.Result as T;
        }

        AsyncOperationHandle<T> loadHandle = Addressables.LoadAssetAsync<T>(address);

        try
        {
            T result = await loadHandle.ToUniTask();

            _handles[address] = loadHandle;
            return result;
        }
        catch(System.Exception e)
        {
            Debug.LogError($" 에셋 로드 실패ㅣ: {address} / Error: {e.Message}");

            if (loadHandle.IsValid())
            {
                Addressables.Release(loadHandle);
            }
            return null;
        }
    }

    public void Release(string address)
    {
        if (_handles.TryGetValue(address, out AsyncOperationHandle handle) == false)
            return;

        Addressables.Release(handle);
        _handles.Remove(address);
        Debug.Log($"에셋 메모리 해체 완료: {address}");
    }

    public void LoadSprite(string address, System.Action<Sprite> callback)
    {
        // 이미 로드된 스프라이트인지 확인 (캐시 활용)
        if (_handles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            callback?.Invoke(handle.Result as Sprite);
            return;
        }

        // 스프라이트 형식으로 로드
        AsyncOperationHandle<Sprite> handleOrigin = Addressables.LoadAssetAsync<Sprite>(address);

        handleOrigin.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[address] = op; // 핸들 저장 (나중에 Release하기 위함)
                callback?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"스프라이트 로드 실패: {address}");
            }
        };
    }

    public async UniTask<Sprite> LoadSprite(string address)
    {
        // 1. 이미 로드된 스프라이트인지 확인 (캐시 활용)
        if (_handles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            // 결과가 Sprite인지 확인 후 반환
            return handle.Result as Sprite;
        }

        // 2. 스프라이트 형식으로 로드 실행
        AsyncOperationHandle<Sprite> handleOrigin = Addressables.LoadAssetAsync<Sprite>(address);

        try
        {
            // ToUniTask()를 통해 비동기 대기
            Sprite result = await handleOrigin.ToUniTask();

            // 3. 핸들 저장 (나중에 Release하기 위함)
            _handles[address] = handleOrigin;

            return result;
        }
        catch (System.Exception)
        {
            // 4. 로드 실패 시 처리
            Debug.LogError($"스프라이트 로드 실패: {address}");

            if (handleOrigin.IsValid())
                Addressables.Release(handleOrigin);

            return null;
        }
    }
}