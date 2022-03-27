using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] int stageSize;

    int currentChipIndex;

    [SerializeField] Transform player;
    [SerializeField] GameObject[] stageChips;
    //  自動生成開始インデックス
    [SerializeField] public int startChipIndex;
    //  先読み個数
    [SerializeField] int preInstantiate;
    [SerializeField] public List<GameObject> generatedStageList = new List<GameObject>();

    private void Start()
    {
        currentChipIndex = startChipIndex - 1;
        UpdateStage(preInstantiate);
    }

    private void Update()
    {
        //  キャラクターの位置から現在のステージチップのインデックスを計算
        int playerPositionIndex = (int)(player.position.x / stageSize);

        //  次のステージチップに入ったら更新処理を行う
        if (playerPositionIndex + preInstantiate > currentChipIndex)
        {
            UpdateStage(playerPositionIndex + preInstantiate);
        }

    }

    //  指定のIndexまでのステージチップを生成して、管理下に置く
    void UpdateStage(int toChipIndex)
    {
        if (toChipIndex <= currentChipIndex) return;

        //　指定のステージチップまで作成
        for (int i = currentChipIndex + 1; i <= toChipIndex; i++)
        {
            GameObject stageObject = GenerateStage(i);

            //  生成したステージチップを管理リストに追加
            generatedStageList.Add(stageObject);
        }

        //  ステージ保持上限内になるまで古いステージを削除
        while (generatedStageList.Count > preInstantiate + 2) DestroyOldestStage();

        currentChipIndex = toChipIndex;
    }

    //  指定のインデックス位置にStageオブジェクトをランダムに生成
    GameObject GenerateStage(int chipIndex)
    {
        int nextStgaeChip = Random.Range(0, stageChips.Length);

        GameObject stageObject = (GameObject)Instantiate(
            stageChips[nextStgaeChip],
            new Vector3(chipIndex * stageSize, 0, 0),
            Quaternion.identity
        );

        return stageObject;
    }

    //  一番古いステージを削除
    void DestroyOldestStage()
    {
        GameObject oldStage = generatedStageList[0];
        generatedStageList.RemoveAt(0);
        Destroy(oldStage);
    }
}