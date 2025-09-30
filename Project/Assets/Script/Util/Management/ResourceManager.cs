using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Util.Debug;
using Backend.Util.Data;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

namespace Backend.Util.Management
{
    /// <summary>
    /// 리소스 기능을 관리 하는 클래스.
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        private ResourceManager()
        {
        }

        private Dictionary<string, UnityEngine.Object> _assets = new();
        private List<Task> _tasks = new();

        private void LoadAssetsByLabelAsync_Internal(string label, string nextLabel)
        {
            // 1. 로드 된 에셋과 로드 할 에셋의 합집합 차집합을 구한다.
            var set = AddressData.Groups[label].Intersect(AddressData.Groups[nextLabel]);   // 현재 로드된 에셋, 로드할 에셋의 합집합
            List<string> set2 = AddressData.Groups[label].Except(set).ToList<string>();   // 로드할 에셋의 차집합  (label - nextLabel): 언로드 목록
            List<string> set3 = AddressData.Groups[nextLabel].Except(set).ToList<string>();   // 로드 된 에셋의 차집합 (nextLabel - label): 로드 목록

            // 2. 언로드 목록에 포함이 되어 있으면 언로드 한다.
            for (int i = set2.Count - 1; i >= 0; i--)
            {
                try
                {
                    Addressables.Release(_assets[set2[i]]);
                }
                catch
                {
                    Debugger.LogMessage("메모리에 없음");
                }
            }

            // 3. 로드목록을 순회하며 비동기 로드 후 _assets에 추가
            for (int i = 0; i < set3.Count; i++)
            {
                AsyncOperationHandle<UnityEngine.Object> handle = Addressables.LoadAssetAsync<UnityEngine.Object>(set3[i]);

                Task task = SetTask(handle, set3[i]); //핸들 작업을 테스크로 보관
                _tasks.Add(task);   //리스트에 추가
            }
        }

        // 각 핸들의 진행 상태가 완료 되면 _assets의 추가 및 진행률 셋팅
        private async Task SetTask(AsyncOperationHandle<UnityEngine.Object> handle, string key)
        {
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GetProgress_Internal();
                _assets[key] = handle.Result;
            }
        }

        private float GetProgress_Internal()
        {
            if (_tasks.Count == 0)
            {
                return 1f;
            }

            int completed = 0;

            // 로딩 중이라면 List 내부의 Task를 순회 하며 로딩 완료 된 Task 개수를 확인
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].IsCompleted)
                {
                    completed++;
                }
            }

            // 로딩 완료된 갯수 / 총 갯수 를 나눠 반환 (퍼센트 구하는 공식)
            Debugger.LogMessage(((float)completed / _tasks.Count).ToString());
            return (float)completed / _tasks.Count;
        }

        private T GetAsset_Internal<T>(string key) where T : UnityEngine.Object
        {
            if (_assets.TryGetValue(key, out UnityEngine.Object obj))
            {
                try
                {
                    return (T)obj;  //변환 해보고 안되면 catch
                }
                catch
                {
                    Debugger.LogMessage($"{key} 주소를 가진 에셋을 {typeof(T).Name} 타입으로 변환할 수 없습니다.");
                    return null;
                }
            }
            else
            {
                Debugger.LogMessage($"{key} 주소를 가진 에셋이 없습니다.");
                return null;
            }
        }

        /// <summary>
        /// 로드 할 씬에 필요한 에셋을 로드 하는 함수.
        /// </summary>
        /// <param name="label">현재 로드 된 씬의 이름.</param>
        /// <param name="nextLabel">로드 할 씬의 이름.</param>
        public static void LoadAssetsByLabelAsync(string label, string nextLabel)
        {
            Instance.LoadAssetsByLabelAsync_Internal(label, nextLabel);
        }

        /// <summary>
        /// 진행률 셋팅 하는 함수.
        /// </summary>
        /// <returns>얼마나 진행 되었는지를 백분율로 반환.</returns>
        public static float GetProgress()
        {
            return Instance.GetProgress_Internal();
        }

        /// <summary>
        /// 필요한 에셋을 지정한 타입으로 받아오기 위한 함수.
        /// </summary>
        /// <typeparam name="T">원하는 타입.</typeparam>
        /// <param name="key">원하는 에셋의 주소.</param>
        /// <returns>필요한 에셋을 지정한 타입으로 반환.</returns>
        public static T GetAsset<T>(string key) where T : UnityEngine.Object
        {
            return Instance.GetAsset_Internal<T>(key);
        }
    }
}
