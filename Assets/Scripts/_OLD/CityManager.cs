/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum HappinessLevel {
    Ecstatic,
    Happy,
    Upset,
    Angry,
}
public class GameClock {
    public int hour;
    public int minute;

    public GameClock(int hour, int minute) {
        this.hour = hour;
        this.minute = minute;
    }
    public override string ToString() {
        return $"{hour:D2}:{minute:D2}";
    }

    public void IncrementMinute() {
        minute++;
        if (minute >= 60) {
            minute = 0;
            hour++;
            if (hour >= 24) {
                hour = 0; // Reset to 0 after reaching 23
            }
        }
    }

}
public class CityManager : MonoBehaviour
{
    public static CityManager Instance;
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Header("Happiness Level Boundaries")]
    // Two contesting values must be outside of the tolerance to be considered imbalanced
    // This makes the early game less punishing
    public int tolerance = 15;
    public int ecstaticMaximum = 2;
    public int happyMaximum = 5;
    public int upsetMaximum = 10;
    public int pointIncrement = 10;
    // Angry is the default, so no maximum needed

    [Header("Happiness Level Penalties")]
    public float ecstaticPenalty = 0f; // No penalty for being ecstatic
    public float happyPenalty = 0.1f;
    public float upsetPenalty = 0.4f;
    public float angryPenalty = 0.9f;

    public float Money {
        get { return money; }
        set {
            money = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    float money = 0f;
    public float Income {
        get { return income; }
        set {
            income = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    float income = 0f;
    public float Population {
        get { return population; }
        set {
            population = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    float population = 0f;
    public float Research {
        get { return research; }
        set {
            research = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    float research = 0f;
    public float Tourism {
        get { return tourism; }
        set {
            tourism = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    float tourism = 0f;
    public float Freight {
        get { return freight; }
        set {
            freight = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    float freight = 0f;
    public float Jobs {
        get { return jobs; }
        set {
            jobs = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    float jobs = 0f;
    public float Shopping {
        get { return shopping; }
        set {
            shopping = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    float shopping = 0f;

    public HappinessLevel Happiness {
        get { return happiness; }
        private set {
            happiness = value;
            HUDBehaviour.Instance.UpdateUI();
        }
    }
    HappinessLevel happiness = 0f;

    public GameClock gameTime = new GameClock(0, 0); // Start at 00:00
    [Header("Game Speed")]
    public float gameSpeed = 1f; // 1x speed by default
    float timer = 0f;

    private void Start() {
        // Initialize the city with some default values
        Money = 3000f;
        Income = 0f;
        Population = 0f;
        Research = 0f;
        Tourism = 0f;
        Freight = 0f;
        Jobs = 0f;
        Shopping = 0f;

        TilemapManager.Instance.OnTileChanged += OnTileChanged;
    }

    void OnTileChanged(TileLayer layer, Vector3Int pos) {
        //if (layer != TileLayer.Building) return;
        //RecalculateBoard();
        *//*TileBase tileBase = TilemapManager.Instance.GetTile(layer, pos);
        if (tileBase == residential) {
            Population += 10f;
        } else if (tileBase == institutional) {
            Research += 10f;
        } else if (tileBase == industrial) {
            Income += 1f;
        } else if (tileBase == recreational) {
            Tourism += 10f;
        } else if (tileBase == commercial) {
            Income += 10f;
        }*//*
    }
    
    void CalculateHappiness() {
        List<string> complaints = new();
        int points = 0;
        // Job - Population Balance
        if (Mathf.Abs(Jobs - Population) > tolerance) {
            var jobPopDeviation = (Mathf.Max(Jobs, Population) / Mathf.Min(Jobs, Population)) - 1f;
            var jpPoints = (int)(jobPopDeviation * 100) / pointIncrement;
            points += jpPoints;
            if (jpPoints > 0) {
                if (Population > Jobs) {
                    complaints.Add("Not enough jobs for the population!");
                }
                else {
                    complaints.Add("Not enough population to fill the jobs!");
                }
            }
        }

        // Poplulation + Tourism - Shopping Balance
        // Tourists do triple the shopping
        var shoppers = Population + (Tourism * 3f);
        if (Mathf.Abs(shoppers - Shopping) > tolerance) {
            var popShopDeviation = (Mathf.Max(shoppers, Shopping) / Mathf.Min(shoppers, Shopping)) - 1f;
            var psPoints = (int)(popShopDeviation * 100) / pointIncrement;
            points += psPoints;
            if (psPoints > 0) {
                if (shoppers > Shopping) {
                    complaints.Add("Not enough shopping for the population and tourists!");
                }
                else {
                    complaints.Add("Not enough population or tourists to support the shopping!");
                }
            }
        }

        // Shopping - Freight Balance
        // It is okay to have excess freight, but not excess shopping
        if (Shopping - Freight > tolerance) {
            var shopFreightDeviation = (Shopping / Freight) - 1f;
            var sfPoints = (int)(shopFreightDeviation * 100) / pointIncrement;
            points += sfPoints;
            if (sfPoints > 0) {
                if (Shopping > Freight) {
                    complaints.Add("Not enough freight to support the shopping!");
                }
                *//*else {
                    complaints.Add("Too much freight for the shopping!");
                }*//*
            }
        }

        // Tourists - Population Balance
        // This is unique in that you can have more population than tourists
        // Each 10 population is worth 1 tourist
        var convertedTourism = Tourism * 10f;
        if(convertedTourism - Population > tolerance) {
            var tourPopDeviation = (convertedTourism /  Population) - 1f;
            var tpPoints = (int)(tourPopDeviation * 100) / pointIncrement;
            points += tpPoints;
            if (tpPoints > 0) {
                if (convertedTourism > Population) {
                    complaints.Add("The population thinks there's too many tourists!");
                }
            }
        }

        if (points < ecstaticMaximum) {
            Happiness = HappinessLevel.Ecstatic;
        }
        else if (points < happyMaximum) {
            Happiness = HappinessLevel.Happy;
        }
        else if (points < upsetMaximum) {
            Happiness = HappinessLevel.Upset;
        }
        else {
            Happiness = HappinessLevel.Angry;
        }

        HappinessButton.Instance.UpdateStatus(complaints);
    }

    public float GetHappinessPenalty() {
        return Happiness switch {
            HappinessLevel.Ecstatic => ecstaticPenalty,
            HappinessLevel.Happy => happyPenalty,
            HappinessLevel.Upset => upsetPenalty,
            HappinessLevel.Angry => angryPenalty,
            _ => 0,
        };
    }

    private void Update() {
    }
}
*/