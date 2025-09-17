using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SPUM_SpriteList : MonoBehaviour
{
    public List<SpriteRenderer> _itemList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _eyeList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _hairList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _bodyList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _clothList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _armorList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _pantList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _weaponList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _backList = new List<SpriteRenderer>();

    public SPUM_HorseSpriteList _spHorseSPList;
    public string _spHorseString;
    // Start is called before the first frame update

    public Texture2D _bodyTexture;
    public string _bodyString;

    public List<string> _hairListString = new List<string>();
    public List<string> _clothListString = new List<string>();
    public List<string> _armorListString = new List<string>();
    public List<string> _pantListString = new List<string>();
    public List<string> _weaponListString = new List<string>();
    public List<string> _backListString = new List<string>();



    public void Reset()
    {
        for (var i = 0; i < _hairList.Count; i++)
        {
            if (_hairList[i] != null) _hairList[i].sprite = null;
        }
        for (var i = 0; i < _clothList.Count; i++)
        {
            if (_clothList[i] != null) _clothList[i].sprite = null;
        }
        for (var i = 0; i < _armorList.Count; i++)
        {
            if (_armorList[i] != null) _armorList[i].sprite = null;
        }
        for (var i = 0; i < _pantList.Count; i++)
        {
            if (_pantList[i] != null) _pantList[i].sprite = null;
        }
        for (var i = 0; i < _weaponList.Count; i++)
        {
            if (_weaponList[i] != null) _weaponList[i].sprite = null;
        }
        for (var i = 0; i < _backList.Count; i++)
        {
            if (_backList[i] != null) _backList[i].sprite = null;
        }
    }
    // public void SetBody(List<Sprite> _mainBodyList , Texture2D mainBody, string bodyString){
    //     _bodyList[0].sprite = _mainBodyList[5];
    //     _bodyList[1].sprite = _mainBodyList[2];
    //     _bodyList[2].sprite = _mainBodyList[0];
    //     _bodyList[3].sprite = _mainBodyList[1];
    //     _bodyList[4].sprite = _mainBodyList[3];
    //     _bodyList[5].sprite = _mainBodyList[4];
    //     _bodyTexture = mainBody;
    //     _bodyString = bodyString;
    // }
    public void LoadSpriteSting()
    {

    }

    public void LoadSpriteStingProcess(List<SpriteRenderer> SpList, List<string> StringList)
    {
        for (var i = 0; i < StringList.Count; i++)
        {
            if (StringList[i].Length > 1)
            {

                // Assets/SPUM/SPUM_Sprites/BodySource/Species/0_Human/Human_1.png
            }
        }
    }

    public void LoadSprite(SPUM_SpriteList data)
    {
        //스프라이트 데이터 연동
        SetSpriteList(_hairList, data._hairList);
        SetSpriteList(_bodyList, data._bodyList);
        SetSpriteList(_clothList, data._clothList);
        SetSpriteList(_armorList, data._armorList);
        SetSpriteList(_pantList, data._pantList);
        SetSpriteList(_weaponList, data._weaponList);
        SetSpriteList(_backList, data._backList);
        SetSpriteList(_eyeList, data._eyeList);

        if (data._spHorseSPList != null)
        {
            SetSpriteList(_spHorseSPList._spList, data._spHorseSPList._spList);
            _spHorseSPList = data._spHorseSPList;
        }
        else
        {
            _spHorseSPList = null;
        }

        //색 데이터 연동.
        if (_eyeList.Count > 2 && data._eyeList.Count > 2)
        {
            _eyeList[2].color = data._eyeList[2].color;
            _eyeList[3].color = data._eyeList[3].color;
        }

        _hairList[3].color = data._hairList[3].color;
        _hairList[0].color = data._hairList[0].color;
        //꺼져있는 오브젝트 데이터 연동.x
        _hairList[0].gameObject.SetActive(!data._hairList[0].gameObject.activeInHierarchy);
        _hairList[3].gameObject.SetActive(!data._hairList[3].gameObject.activeInHierarchy);

        _hairListString = data._hairListString;
        _clothListString = data._clothListString;
        _pantListString = data._pantListString;
        _armorListString = data._armorListString;
        _weaponListString = data._weaponListString;
        _backListString = data._backListString;
    }

    public void SetSpriteList(List<SpriteRenderer> tList, List<SpriteRenderer> tData)
    {
        for (var i = 0; i < tData.Count; i++)
        {
            if (tData[i] != null)
            {
                tList[i].sprite = tData[i].sprite;
                tList[i].color = tData[i].color;
            }
            else tList[i] = null;
        }
    }

    public void ResyncData()
    {
        SyncPath(_hairList, _hairListString);
        SyncPath(_clothList, _clothListString);
        SyncPath(_armorList, _armorListString);
        SyncPath(_pantList, _pantListString);
        SyncPath(_weaponList, _weaponListString);
        SyncPath(_backList, _backListString);
    }

    public void SyncPath(List<SpriteRenderer> _objList, List<string> _pathList)
    {
        for (var i = 0; i < _pathList.Count; i++)
        {
            if (_pathList[i].Length > 1)
            {
                string tPath = _pathList[i];
                tPath = tPath.Replace("Assets/Resources/", "");
                tPath = tPath.Replace(".png", "");

                Sprite[] tSP = Resources.LoadAll<Sprite>(tPath);
                if (tSP.Length > 1)
                {
                    if (_objList[0].name == "ClothBody" || _objList[0].name == "BodyArmor")
                    {
                        string tmpName = "";
                        switch (i)
                        {
                            case 0:
                                tmpName = "Body";
                                break;

                            case 1:
                                tmpName = "Left";
                                break;

                            case 2:
                                tmpName = "Right";
                                break;
                        }

                        foreach (Sprite ttS in tSP)
                        {
                            if (ttS.name == tmpName)
                            {
                                _objList[i].sprite = ttS;
                                break;
                            }
                        }
                    }

                }
                else if (tSP.Length > 0)
                {
                    _objList[i].sprite = tSP[0];
                }
            }
            else
            {
                _objList[i].sprite = null;
            }
        }
    }
    // 이 어트리뷰트 덕분에 인스펙터의 톱니바퀴 메뉴에서 이 함수를 클릭해 실행할 수 있습니다.
    [ContextMenu("Find & Assign Parts Automatically")]
    public void AutoAssignParts()
    {
        // 시작하기 전에 모든 리스트를 깨끗하게 비웁니다.
        _hairList.Clear();
        _clothList.Clear();
        _armorList.Clear();
        _pantList.Clear();
        _weaponList.Clear();
        _backList.Clear();
        _eyeList.Clear();
        _bodyList.Clear();

        // 이 게임 오브젝트와 모든 자식들로부터 SpriteRenderer 컴포넌트를 전부 찾아옵니다.
        // true를 인자로 주면 비활성화된 오브젝트도 포함해서 찾습니다.
        SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // 찾은 모든 렌더러들을 순회하면서 이름 규칙에 따라 적절한 리스트에 추가합니다.
        foreach (SpriteRenderer renderer in allRenderers)
        {
            string objectName = renderer.name;

            if (objectName.StartsWith("Hair"))
            {
                _hairList.Add(renderer);
            }
            else if (objectName.StartsWith("Cloth"))
            {
                _clothList.Add(renderer);
            }
            else if (objectName.StartsWith("Armor"))
            {
                _armorList.Add(renderer);
            }
            else if (objectName.StartsWith("Pant"))
            {
                _pantList.Add(renderer);
            }
            else if (objectName.StartsWith("Weapon"))
            {
                _weaponList.Add(renderer);
            }
            else if (objectName.StartsWith("Back"))
            {
                _backList.Add(renderer);
            }
            else if (objectName.StartsWith("Eye"))
            {
                _eyeList.Add(renderer);
            }
            else if (objectName.StartsWith("Body"))
            {
                _bodyList.Add(renderer);
            }
            // 필요한 다른 파츠들도 여기에 추가할 수 있습니다.
        }

        // 정렬이 중요하다면 이름순으로 정렬하는 로직을 추가할 수 있습니다.
        // 예: _clothList = _clothList.OrderBy(r => r.name).ToList();

        // 에디터에서 변경사항을 즉시 저장하기 위해 사용합니다. (선택적)
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        Debug.Log("모든 파츠를 자동으로 찾아 할당했습니다!");
    }
}
