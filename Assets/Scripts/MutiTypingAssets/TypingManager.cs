using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

/* タイピングの制御を行うスクリプト */
public class TypingManager : MonoBehaviour
{
    /*タイピングの機能に関しての変数*/

    // 画面にあるテキストを持ってくる
    private Text qText; // 問題用のテキスト
    private Text fText; // ふりがな用のテキスト 
    private Text aText; // 答え用のテキスト

    // テキストデータを読み込む
    [SerializeField] TextAsset _furigana;
    [SerializeField] TextAsset _question;
    // 英語ver
    [SerializeField] TextAsset englishTextData;

    // テキストデータを格納するためのリスト
    private List<string> _fList = new List<string>();
    private List<string> _qList = new List<string>();
    // 英語ver
    private List<string> _eList = new List<string>();


    // 何番目か指定するためのstring
    private string _fString;
    private string _qString;
    private string _aString;
    // 英語ver
    private string _eString;

    // 何番目の問題か
    private int _qNum;

    // 問題の何文字目か
    private int _aNum;

    // 一つ前の問題番号
    int oldNumOfQuestion;

    // 合ってるかどうかの判断
    bool isCorrect;
    // Shitキーを押しているかどうか
    bool isShift = false;

    private ChangeDictionary cd;

    // しんぶん→"si","n","bu","n"
    // しんぶん→"shi","n","bu","n"
    // {0,0,1,2,2,3}　_furiCountList
    // {0,1,0,0,1,0}　_romNumList
    private List<string> _romSliceList  = new List<string>();
    private List<int>    _furiCountList = new List<int>();
    private List<int>    _romNumList    = new List<int>();

    // 何文字ひらがなを成功したか
    int hiraCount = 0;

    /*Playerに関する変数*/
    private Player        player;
    private KingPlayer    kingPlayer;
    private WizzardPlayer wizzardPlayer;

    //　正解数
    public int correctN;
    //　失敗数
    public int mistakeN;
    //　正解率
    public float correctAR;

    // 省略用の変数
    GameManager gameManager;

    // ゲームを始めた時に1度だけ呼ばれるもの
    void Start()
    {
        gameManager = GameManager.instance;

        //　テキストUIを取得
        qText = transform.Find("InputPanel/QuestionJ").GetComponent<Text>();
        fText = transform.Find("InputPanel/QuestionR").GetComponent<Text>();
        aText = transform.Find("InputPanel/Input").GetComponent<Text>();

        cd = GetComponent<ChangeDictionary>();
        
        SetPlayer(gameManager.playerType);

        // テキストデータをリストに入れる
        SetList();

        // 問題を出す
        OutPut();
    }

    void SetPlayer(GameManager.PlayerType playerType)
    {
        switch (playerType)
        {
            case GameManager.PlayerType.KNIGHT:
                player        = GameObject.Find("Player").GetComponent<Player>();
                break;
            case GameManager.PlayerType.KINGPLAYER:
                kingPlayer    = GameObject.Find("KingPlayer").GetComponent<KingPlayer>();
                break;
            case GameManager.PlayerType.WIZZARDPLAYER:
                wizzardPlayer = GameObject.Find("WizzardPlayer").GetComponent<WizzardPlayer>();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) return;
        // WizzardPlayerのためのスペースとエンターキーは無視
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) return;

        // 入力された時に判断する
        if (Input.anyKeyDown && !gameManager.isEnglish && !gameManager.isEnemyDead)
        {
            isCorrect = false;
            int furiCount = _furiCountList[_aNum];

            // 入力したキーをDebug.Logで確認する
            /*foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    Debug.Log(code);
                    break;
                }
            }*/

            // 完全に合ってたら正解！
            // し s  i
            if (Input.GetKeyDown(_aString[_aNum].ToString()) || GetKeyReturn() == _aString[_aNum].ToString())
            {
                // trueにする
                isCorrect = true;

                // 正解
                Correct();

                // 最後の文字に正解したら
                if (_aNum >= _aString.Length)
                {
                    // Playerが攻撃を行う
                    if (gameManager.playerType == GameManager.PlayerType.KNIGHT)
                    {
                        player.Attack();
                    }
                    // 問題を変える
                    OutPut();
                }
            }
            else if (Input.GetKeyDown("n") && furiCount > 0 && _romSliceList[furiCount - 1] == "n")
            {
                // nnにしたい
                _romSliceList[furiCount - 1] = "nn";
                _aString = string.Join("", GetRomSliceListWithoutSkip());

                ReCreateList(_romSliceList);

                // trueにする
                isCorrect = true;

                // 正解
                Correct();

                // 最後の文字に正解したら
                if (_aNum >= _aString.Length)
                {
                    if (gameManager.playerType == GameManager.PlayerType.KNIGHT)
                    {
                        player.Attack();
                    }
                    // 問題を変える
                    OutPut();
                }
            }
            else
            {
                // し →si, ci, shi
                // 柔軟な入力があるかどうか
                // 「し」-> "si" , "shi"
                // 今どの ふりがな を打たないといけないのかを取得する
                string currentFuri = _fString[furiCount].ToString();

                if (furiCount < _fString.Length - 1)
                {
                    // 2文字を考慮した候補検索「しゃ」
                    string addNextMoji = _fString[furiCount].ToString() + _fString[furiCount + 1].ToString();
                    CheckIrregukarType(addNextMoji, furiCount, false);
                }

                if (!isCorrect)
                {
                    // 今まで通りの候補検索「し」「ゃ」
                    string moji = _fString[furiCount].ToString();
                    CheckIrregukarType(moji, furiCount, true);
                }   
            }

            // 正解じゃなかったらマウスの入力は受け付けない
            if (!isCorrect && Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                // 失敗
                Miss();
            }
        }
        else if(gameManager.isEnglish && !gameManager.isEnemyDead) // 英語版
        {
            // もし問題が英字の大文字だったら
            if (IsUpperString(_aString[_aNum].ToString()))
            {
                // Shiftと同時押しだったら正しい
                if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(_aString[_aNum].ToString().ToLower()))
                {
                    Correct();
                }
                // それ以外（マウスの入力は受け付けない）
                else if(Input.anyKeyDown && !Input.GetMouseButtonDown(0) &&
                !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                {
                    Miss();
                }
            }
            //　今見ている文字とキーボードから打った文字が同じかどうか
            else if (Input.GetKeyDown(_aString[_aNum].ToString()))
            {
                //　正解時の処理を呼び出す
                Correct();
                //　問題を入力し終えたら次の問題を表示
                if (_aNum >= _aString.Length)
                {
                    OutPut();
                    if (gameManager.playerType == GameManager.PlayerType.KNIGHT)
                    {
                        player.Attack();
                    }                 
                }
            }
            // マウスの入力は受け付けない
            else if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) &&
                !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                //　失敗時の処理を呼び出す
                Miss();
            }
        }
    }

    bool IsUpperString(string s)
    {
        // 指定した文字列が大文字かどうかを判定します。
        if (string.IsNullOrEmpty(s)) { return false; }

        foreach (char c in s)
        {
            if (!Char.IsUpper(c))
            {
                return false;
            }
        }
        return true;
    }

    void CheckIrregukarType(string currentFuri, int furiCount, bool addSmallMoji)
    {
        if (cd.dic.ContainsKey(currentFuri))
        {
            List<string> stringList = cd.dic[currentFuri]; // ci, shi
            //Debug.Log(string.Join(",", stringList));

            // stringList[0] ci, stringList[1] shi
            for (int i = 0; i < stringList.Count; i++)
            {
                string rom = stringList[i];
                int romNum = _romNumList[_aNum];

                bool preCheck = true;

                for (int j = 0; j < romNum; j++)
                {
                    if (rom[j] != _romSliceList[furiCount][j])
                    {
                        preCheck = false;
                    }
                }

                if (preCheck && Input.GetKeyDown(rom[romNum].ToString()))
                {
                    _romSliceList[furiCount] = rom;
                    _aString = string.Join("", GetRomSliceListWithoutSkip());

                    ReCreateList(_romSliceList);

                    // trueにする
                    isCorrect = true;

                    if (addSmallMoji)
                    {
                        AddSmallMoji();
                    }

                    // 正解
                    Correct();

                    // 最後の文字に正解したら
                    if (_aNum >= _aString.Length)
                    {
                        if (gameManager.playerType == GameManager.PlayerType.KNIGHT)
                        {
                            player.Attack();
                        }
                        // 問題を変える
                        OutPut();
                    }
                    break;
                }
            }
        }

    }

    void SetList()
    {
        string[] _fArray = _furigana.text.Split('\n');
        _fList.AddRange(_fArray);

        string[] _qArray = _question.text.Split('\n');
        _qList.AddRange(_qArray);
        // 英語用
        string[] _eArray = englishTextData.text.Split('\n');
        _eList.AddRange(_eArray);       
    }

    // 柔軟な入力をしたときに次の文字が小文字なら小文字を挿入する
    void AddSmallMoji()
    {
        int nextMojiNum = _furiCountList[_aNum] + 1;

        // もし次の文字がなければ処理をしない
        if (_fString.Length - 1 < nextMojiNum)
        {
            return;
        }

        string nextMoji = _fString[nextMojiNum].ToString();
        string a = cd.dic[nextMoji][0];

        // もしaの0番目がxでもlでもなければ処理をしない
        if (a[0] != 'x' && a[0] != 'l')
        {
            return;
        }
        else if (nextMoji == "っ")
        {
            return;
        }

        // romslicelistに挿入と表示の反映
        _romSliceList.Insert(nextMojiNum, a);
        // SKIPを削除する
        _romSliceList.RemoveAt(nextMojiNum + 1);

        // 変更したリストを再度表示させる
        ReCreateList(_romSliceList);
        _aString = string.Join("", GetRomSliceListWithoutSkip());
    }

    // しんぶん→"shi","n","bu","n"
    // { 0, 0, 1, 2, 2, 3 }
    // { 0, 1, 0, 0, 1, 0 }
    void CreatRomSliceList(string moji)
    {
        _romSliceList.Clear();
        _furiCountList.Clear();
        _romNumList.Clear();

        // 「し」→「si」,「ん」→「n」
        for (int i = 0; i < moji.Length; i++)
        {
            string a = cd.dic[moji[i].ToString()][0];

            // ①後ろの文字が「あ行」または「な行」の時　と　②最後の文字の時　の　「ん」=> 「nn」
            if (i + 1 < moji.Length && moji[i].ToString() == "ん" && (moji[i + 1].ToString() == "あ" || moji[i + 1].ToString() == "い" || moji[i + 1].ToString() == "う"
                || moji[i + 1].ToString() == "え" || moji[i + 1].ToString() == "お" || moji[i + 1].ToString() == "な"
                || moji[i + 1].ToString() == "に" || moji[i + 1].ToString() == "ぬ" || moji[i + 1].ToString() == "ね"
                || moji[i + 1].ToString() == "の"))
            {
                a = "nn";
            }

            if (moji[i].ToString() == "ん" && i == moji.Length - 1)
            {
                a = "nn";
            }

            if (moji[i].ToString() == "ゃ" || moji[i].ToString() == "ゅ" || moji[i].ToString() == "ょ"
                || moji[i].ToString() == "ぁ" || moji[i].ToString() == "ぃ" || moji[i].ToString() == "ぅ"
                || moji[i].ToString() == "ぇ" || moji[i].ToString() == "ぉ")
            {
                a = "SKIP";
            }
            else if (moji[i].ToString() == "っ" && i + 1 < moji.Length)
            {
                a = cd.dic[moji[i + 1].ToString()][0][0].ToString();
            }
            else if (i + 1 < moji.Length)
            {
                // 次の文字も含めて辞書から探す
                string addNextMoji = moji[i].ToString() + moji[i + 1].ToString();
                if (cd.dic.ContainsKey(addNextMoji))
                {
                    a = cd.dic[addNextMoji][0];
                }
            }

            _romSliceList.Add(a);

            if (a == "SKIP")
            {
                continue;
            }
            for (int j = 0; j < a.Length; j++)
            {
                _furiCountList.Add(i);
                _romNumList.Add(j);
            }
        }
        //Debug.Log(string.Join(",", _romSliceList));
    }

    // 複数入力、イレギュラー入力の際にromSliceListのFuriCountやRomNumを変更する関数
    void ReCreateList(List<string> romList)
    {
        _furiCountList.Clear();
        _romNumList.Clear();
        for (int i = 0; i < romList.Count; i++)
        {
            string a = romList[i];
            if (a == "SKIP")
            {
                continue;
            }
            for (int j = 0; j < a.Length; j++)
            {
                _furiCountList.Add(i);
                _romNumList.Add(j);
            }
        }
        //Debug.Log(string.Join(",", _romSliceList));
    }

    // SKIPなしの表示をさせるためのListを作り直す
    List<string> GetRomSliceListWithoutSkip()
    {
        List<string> returnList = new List<string>();
        foreach (string rom in _romSliceList)
        {
            if (rom == "SKIP")
            {
                continue;
            }
            returnList.Add(rom);
        }
        return returnList;
    }

    // Shiftを押した時
    string GetKeyReturn()
    {
        isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isShift)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                return "!";
            }
        }      
        return "";
    }

    // 問題を出すための関数
    public void OutPut()
    {
        // 0番目の文字に戻す
        _aNum = 0;
        
        // _qNumに０〜問題数の数までのランダムな数字を1つ入れる
        _qNum = UnityEngine.Random.Range(0, _qList.Count);

        //  問題が重複しないように設定		
        if (oldNumOfQuestion == _qNum)
        {
            // 重複した問題番号は乱数で加算する
            _qNum = _qNum + UnityEngine.Random.Range(1, _qList.Count);
            //　加算した問題番号がqJ.Lengthを超えたら(qJ.Length + 1)を引いて数を合わせる
            if (_qNum > _qList.Count)
            {
                _qNum = _qNum - (_qList.Count);
            }
        }

        if (gameManager.isEnglish)
        {
            // 英語版（複数入力がないので_aStringにそのまま英字を代入）
            _fString = _eList[_qNum];
            _qString = _qList[_qNum];

            _aString = _eList[_qNum];
        }
        else
        {
            _fString = _fList[_qNum];
            _qString = _qList[_qNum];

            CreatRomSliceList(_fString);
            _aString = string.Join("", GetRomSliceListWithoutSkip());
        }
        
        // 古い問題番号を記録
        oldNumOfQuestion = _qNum;

        // 文字を変更する
        fText.text = _fString;
        qText.text = _qString;
        aText.text = _aString;
    }

    // 正解用の関数
    void Correct()
    {
        // 正解した時の効果音
        SoundManager.instance.PlaySE(11);
        // 正解数を増やす
        correctN++;
        //　正解率の計算
        CorrectAnswerRate();

        // KingPlayerのAttackDamageを増やす
        if (gameManager.playerType == GameManager.PlayerType.KINGPLAYER)
        {
            kingPlayer.IncrementChargeAttackDamage();
        }
        // wizzardPlayerのMagicPowerを増やす
        else if (gameManager.playerType == GameManager.PlayerType.WIZZARDPLAYER)
        {
            wizzardPlayer.IncrementMagicPower();
        }
        // 正解した時の処理（やりたいこと）
        _aNum++;
        aText.text = "<color=#6A6A6A>" + _aString.Substring(0, _aNum) + "</color>" + _aString.Substring(_aNum);
        //Debug.Log(_aNum);

        // 今のひらがなのfuriCountが1個前のfuriCountと違ったら
        if (_aNum >= _furiCountList.Count || _furiCountList[_aNum] != _furiCountList[_aNum - 1])
        {
            hiraCount++;
        }
    }

    // 間違え用の関数
    void Miss()
    {
        // 失敗した時の効果音
        SoundManager.instance.PlaySE(12);
        //　失敗数を増やす（同時押しにも対応させる）
        mistakeN++;

        // missするとchargeAtttackDamageが減少
        if (gameManager.playerType == GameManager.PlayerType.KINGPLAYER)
        {
            kingPlayer.DecrementChargeAttackDamage();
        }
        // missするとmagicPowerが減少
        else if (gameManager.playerType == GameManager.PlayerType.WIZZARDPLAYER)
        {
            wizzardPlayer.DecrementMagicPower();
        }
        // 間違えた時にやりたいこと
        aText.text = "<color=#6A6A6A>" + _aString.Substring(0, _aNum) + "</color>"
            + "<color=#FF0000>" + _aString.Substring(_aNum, 1) + "</color>"
            + _aString.Substring(_aNum + 1);
    }

    //　正解率の計算処理
    void CorrectAnswerRate()
    {
        //　正解率の計算
        correctAR = 100f * correctN / (correctN + mistakeN);
    }

    
}