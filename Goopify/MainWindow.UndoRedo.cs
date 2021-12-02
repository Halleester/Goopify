using System;

namespace Goopify
{
    public class MainWindowUndoRedo: Form
    {

        public override void WindowConstructor()
        {
            undoStack.Push(new RegionState()); // Push empty goop region to stack to start
            base.WindowConstructor();
        }
        // Undo/Redo stuff

        private class RegionState
        {
            public GoopRegionBox[] selectedRegions;
            public GoopRegionBox[] currentRegions;

            /// <summary>
            /// Creates a deep clone of the gotten regions and saves them to this RegionState
            /// </summary>
            /// <param name="allRegions">All goop regions</param>
            /// <param name="currentSelectedRegions">The selected goop regions</param>
            public RegionState(List<GoopRegionBox> allRegions, List<GoopRegionBox> currentSelectedRegions)
            {
                int selectedRegionIndex = 0;
                selectedRegions = new GoopRegionBox[currentSelectedRegions.Count];
                currentRegions = new GoopRegionBox[allRegions.Count];
                for (int i = 0; i < allRegions.Count; i++)
                {
                    GoopRegionBox clonedRegion = allRegions[i].Clone();
                    currentRegions[i] = clonedRegion;
                    if (currentSelectedRegions.Contains(allRegions[i]))
                    {
                        Console.WriteLine("Got selected region");
                        selectedRegions[selectedRegionIndex] = clonedRegion;
                        selectedRegionIndex++;
                    }
                }
            }

            public RegionState(RegionState existingState)
            {
                List<GoopRegionBox> allRegions = existingState.currentRegions.ToList();
                List<GoopRegionBox> currentSelectedRegions = existingState.selectedRegions.ToList();

                int selectedRegionIndex = 0;
                selectedRegions = new GoopRegionBox[currentSelectedRegions.Count];
                currentRegions = new GoopRegionBox[allRegions.Count];
                for (int i = 0; i < allRegions.Count; i++)
                {
                    GoopRegionBox clonedRegion = allRegions[i].Clone();
                    currentRegions[i] = clonedRegion;
                    if (currentSelectedRegions.Contains(allRegions[i]))
                    {
                        Console.WriteLine("Got selected region");
                        selectedRegions[selectedRegionIndex] = clonedRegion;
                        selectedRegionIndex++;
                    }
                }
            }

            public RegionState()
            {
                selectedRegions = new GoopRegionBox[0];
                currentRegions = new GoopRegionBox[0];
            }
        }

        private Stack<RegionState> undoStack = new Stack<RegionState>();
        private Stack<RegionState> redoStack = new Stack<RegionState>();
    }
}
