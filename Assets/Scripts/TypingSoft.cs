using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingSoft : MonoBehaviour
{
	//　問題の日本語文
	private string[] qJ = { "問題", "テスト", "タイピング", " 学校", "病院", "空港", "工場", "大学",
		                    "宝物", "プログラミング", "経済学", "統計学", "動物", "結婚", " タンパク質",
		                   "心理学", "成分", "犠牲", "評判", "特徴", "運命", "炎", "建築", "挨拶", "天気",
							"枕", "鏡", "頭痛", "攻撃", "戦争", "宇宙", "自然", "キャンプ", "使命", "英雄",
							"使用人"};
	//　問題のローマ字文
	private string[] qR = { "monndai", "tesuto", "taipinngu", "gakkou", "byouinn", "kuukou", "kouzyou", "daigaku",
		　　　　　　　　　　　"takaramono", "puroguraminngu", "keizaigaku", "toukeigaku", "doubutu", "kekkonn", "tannpakusitu",
		                 "sinnrigaku", "seibunn", "gisei", "hyoubann", "tokutyou", "unnmei", "honoo", "kenntiku", "aisatu", "tennki",
							"makura", "kagami", "zutuu", "kougeki", "sennsou", "utyuu", "sizenn", "kyannpu", "simei", "eiyuu",
							"siyouninn"};
	// 問題文の英語文
	private string[] qE = { "question", "test", "typing", "school", "hospital", "airport", "factory", "university",
		                   "treasure", "programming", "economics", "statistics", "animal","marriage", "protein",
                           "psychology", "ingredient", "sacrifice", "reputation", "feature", "fate", "blaze", "architecture", "greeting", "weather",
							"pillow", "mirror", "headache", "attack", "war", "universe", "nature", "camp", "mission", "hero",
							"servant"};
	//　日本語表示テキスト
	private Text UIJ;
	//　ローマ字表示テキスト
	private Text UIR;
	//　日本語問題
	private string nQJ;
	//　ローマ字問題
	private string nQR;
	//　問題番号
	private int numberOfQuestion;
	// 一つ前の問題番号
	private int oldNumOfQuestion;
	//　問題は何文字目か
	private int indexOfString;
	
 
	//　入力した文字列テキスト
	private Text UII;
	//　正解数
	public int correctN;
	//　正解した文字列を入れておく
	private string correctString;
	//　失敗数
	public int mistakeN;
	//　正解率
	public float correctAR;

	private GameObject player;
	
	void Start()
	{
		player = GameObject.Find("Player");
		//　テキストUIを取得
		UIJ = transform.Find("InputPanel/QuestionJ").GetComponent<Text>();
		UIR = transform.Find("InputPanel/QuestionR").GetComponent<Text>();
		UII = transform.Find("InputPanel/Input").GetComponent<Text>();
		

		//　データ初期化処理
		correctN = 0;
		mistakeN = 0;
		correctAR = 0;

		//　問題出力メソッドを呼ぶ
		OutputQ();
	}

	
	//　新しい問題を表示するメソッド
	void OutputQ()
	{
		//　テキストUIを初期化する
		UIJ.text = "";
		UIR.text = "";
		UII.text = "";

		//　正解した文字列を初期化
		correctString = "";
		//　文字の位置を0番目に戻す
		indexOfString = 0;

		//　問題数内でランダムに選ぶ
		numberOfQuestion = Random.Range(0, qJ.Length);

		//  問題が重複しないように設定		
		if (oldNumOfQuestion == numberOfQuestion)
        {
			// 重複した問題番号は乱数で加算する
			numberOfQuestion = numberOfQuestion + Random.Range(1, qJ.Length);
			//　加算した問題番号がqJ.Lengthを超えたら(qJ.Length + 1)を引いて数を合わせる
			if (numberOfQuestion > qJ.Length)
            {
				numberOfQuestion = numberOfQuestion - (qJ.Length + 1);
            }
        }

		//　選択した問題をテキストUIにセット
		nQJ = qJ[numberOfQuestion];

		//  古い問題番号を取得
		oldNumOfQuestion = numberOfQuestion;


		// ステージ選択画面で日本語か英語どちらを選んだか
		if (GameManager.instance.isEnglish)
		 {
			nQR = qE[numberOfQuestion];
		 }
        else
        {
			nQR = qR[numberOfQuestion];
		}
		UIJ.text = nQJ;
		UIR.text = nQR;
	}

	
	void Update() {

		/*複数入力の場合、修正必要*/
		//　今見ている文字とキーボードから打った文字が同じかどうか
		if (Input.GetKeyDown(nQR[indexOfString].ToString()))
		{
			//　正解時の処理を呼び出す
			Correct();
			//　問題を入力し終えたら次の問題を表示
			if (indexOfString >= nQR.Length)
			{     
				OutputQ();
				player.SendMessage("Attack");
			}
		}
		else if (Input.anyKeyDown)
		{
			//　失敗時の処理を呼び出す
			Mistake();
		}
	}
	
	//　タイピング正解時の処理
    void Correct() {
		//　正解数を増やす
		correctN++;
		//　正解率の計算
		CorrectAnswerRate();
		//　正解した文字を表示
		correctString += nQR[indexOfString].ToString();
		UII.text = correctString;
		//　次の文字を指す
		indexOfString++;
	}

	//　タイピング失敗時の処理
	void Mistake()
	{
		//　失敗数を増やす（同時押しにも対応させる）
		mistakeN += Input.inputString.Length;
		//　失敗した文字を表示
		if (Input.inputString != "")
		{
			UII.text = correctString + "<color=#ff0000ff>" + Input.inputString + "</color>";
		}
	}

	//　正解率の計算処理
	void CorrectAnswerRate()
	{
		//　正解率の計算
		correctAR = 100f * correctN / (correctN + mistakeN);
	}
}
