using System.Collections;

public class DummyIUserInterface : IUserInput
{
    // Start is called before the first frame update
    IEnumerator Start(){
        // dirUpOrigin = 1.0f;
        // dirRightOrigin = 0;
        while (true) {
            rb = true;
            yield return 0;
        }
        
    }

    // Update is called once per frame
    void Update(){
        CalculateDmagDvec(dirUpOrigin,dirRightOrigin);
    }
}
