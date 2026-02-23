namespace ConsoleApp;

public class PostcodeEnumerator
{
    private static readonly string[][] ComponentValues =
    [
        ["I"],
        ["M"],
        ["1", "2", "3", "4", "5", "6", "7", "8", "9", "86", "87", "99"],
        ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"],
        ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"],
        ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"],
    ];
    
    public IEnumerable<string> EnumeratePostcodes(string startingPostcode)
    {
        int[] componentPositions = MapToComponentIndexes(startingPostcode);

        while (true)
        {
            int selectedComponent = ComponentValues.Length - 1;
            yield return string.Join("", componentPositions.Select((pos, component) => ComponentValues[component][pos]));

            // increment the value on the current (right-most) position
            componentPositions[selectedComponent]++;
            
            // if it was the last value in that position, a "carry-over" is needed
            while(true)
            {
                if (componentPositions[selectedComponent] < ComponentValues[selectedComponent].Length)
                {
                    // we're not on the last value - clear positions to the right
                    // (usually nothing to clear, but if there was a carry-over from a previous position, it needs to be cleared)
                    for (int i = selectedComponent+1; i < ComponentValues.Length; i++)
                    {
                        componentPositions[i] = 0;
                    }
                    break;
                }

                // carry-over is happening
                // move to the left-side position (which is the next position to change)
                selectedComponent--;
                if (selectedComponent < 0)
                {
                    // all positions have been used, finish enumeration
                    goto after_main_loop;
                }

                // increment the value on this new position
                componentPositions[selectedComponent]++;
            }
        }

        after_main_loop: ;
    }

    private int[] MapToComponentIndexes(string startingPostcode)
    {
        string[] components = startingPostcode.Length switch
        {
            6 =>
            [
                startingPostcode[0..1], startingPostcode[1..2], startingPostcode[2..3],
                startingPostcode[3..4], startingPostcode[4..5], startingPostcode[5..6],
            ], 
            7 =>
            [
                startingPostcode[0..1], startingPostcode[1..2], startingPostcode[2..4],
                startingPostcode[4..5], startingPostcode[5..6], startingPostcode[6..7],
            ],
            _ => throw new ArgumentException("Invalid postcode format")
        };
        int[] indexes = new int[ComponentValues.Length];
        for (int i = 0; i < ComponentValues.Length; i++)
        {
            indexes[i] = Array.IndexOf(ComponentValues[i], components[i]);
            if (indexes[i] == -1)
            {
                throw new ArgumentException($"Invalid component value '{components[i]}' in postcode");
            }
        }
        return indexes;
    }
}