using UnityEngine;
using System.Collections;


public class AI_NathanWeisskopf : MonoBehaviour {

    public CharacterScript mainScript;

    public float[] bombSpeeds;
    public float[] buttonCooldowns;
    public float playerSpeed;
    public int[] beltDirections;
    public float[] buttonLocations;

	// Use this for initialization
	void Start () {
        mainScript = GetComponent<CharacterScript>();

        if (mainScript == null)
        {
            print("No CharacterScript found on " + gameObject.name);
            this.enabled = false;
        }

        buttonLocations = mainScript.getButtonLocations();

        playerSpeed = mainScript.getPlayerSpeed();
	}

    int targetBelt = 0;

    // Update is called once per frame
    void Update () {
       
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();

        //Your AI code goes here

        // Keeping track of best decision & belt index of best decision
        int bestH = calculateH(beltDirections[0], buttonCooldowns[0], 0);
        int bestIndex = 0;

        for (int i = 1; i < beltDirections.Length; i++)
        {
            int tempH = calculateH(beltDirections[i], buttonCooldowns[i], i);
            if (tempH > bestH) {
                bestH = tempH;
                bestIndex = i;
            }
        }

        // Moving towards best belt to push button
        targetBelt = bestIndex;

        if (buttonLocations[targetBelt] < mainScript.getCharacterLocation())
        {
            mainScript.moveDown();
            mainScript.push();
        } else
        {
            mainScript.moveUp();
            mainScript.push();
        }



	}

    /// <summary>
    /// Returns a heuristic based on a belt's direction and button cooldown, used to determine best belt to target
    /// </summary>
    /// <param name="beltdirection"></param>
    /// <param name="buttoncooldown"></param>
    /// <returns></returns>
    int calculateH (int beltdirection, float buttoncooldown, int beltNum) {
        int h = 0;

        print(beltdirection);

        // Attributing incentive to distance from enemy player
        float locationDifference = (Mathf.Abs(mainScript.getCharacterLocation() - mainScript.getOpponentLocation()));
        float playerProximity = (Mathf.Abs(mainScript.getCharacterLocation() - buttonLocations[beltNum]));

        // Attributing incentive to player position vs opponent position
        if (locationDifference < 1.0)
        {
            h -= 100;
        }
        else if (locationDifference < 2.0)
        {
            h -= 50;
        }
        else if (locationDifference < 4.0)
        {
            h -= 25;
        }
        else if (locationDifference < 8.0)
        {
            h += 25;
        }
        else if (locationDifference < 12.0)
        {
            h += 50;
        }
        else if (locationDifference < 16.0)
        {
            h += 100;
        }

        // Attributing incentive to belt movemoment
        if (beltdirection == -1)
        {
            if ((locationDifference < 2.0))
            {
                h += 10;
            } else if (locationDifference < 4.0)
            {
                h += 25;
            } else if (locationDifference < 8.0)
            {
                h += 50;
            } else if (locationDifference < 12.0)
            {
                h += 100;
            } else if (locationDifference < 16.0)
            {
                h += 150;
            }

            // If player is near a moving belt, attribute priority
            if (playerProximity < 1.0)
            {
                h += 150;
            } else if (playerProximity < 2.0)
            {
                h += 100;
            } else if (playerProximity < 4.0)
            {
                h += 50;
            } else if (playerProximity < 8.0)
            {
                h += 25;
            }
        } else if (beltdirection == 0)
        {
            if ((locationDifference < 2.0))
            {
                h += 5;
            }
            else if (locationDifference < 4.0)
            {
                h += 10;
            }
            else if (locationDifference < 8.0)
            {
                h += 15;
            } 
            else if (locationDifference < 12.0)
            {
                h += 25;
            }
            else if (locationDifference < 16.0)
            {
                h += 50;
            }
        } else 
        {
            // Belt already moving in player favor, no reason to interact
            h -= 10000;
        }

        // Attributing incentive to button cooldowns
        if (buttoncooldown > 0.9)
        {
            h -= 500;
        } else if (buttoncooldown > 0.75)
        {
            h -= 250;
        } else if (buttoncooldown > 0.5)
        {
            h -= 100;
        } else if (buttoncooldown > 0.25)
        {
            h -= 50;
        } else if (buttoncooldown == 0)
        {
            h += 50;
        }

        // Avoiding proximity to enemy player
        float enemyProx = (Mathf.Abs(buttonLocations[beltNum] - mainScript.getOpponentLocation()));
        if (enemyProx < 1.0)
        {
            h -= 50;
        } else if (enemyProx < 2.0)
        {
            h -= 25;
        }

        return h;
    }
}
