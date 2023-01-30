
/*
 * https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html
 * 
 * 
 * 
 *  Debug.Log($"<color=red></color>");      this is for debug checking
 * 
 *  Debug.Log($"<color=orange></color>");      this is for debug checking
 *  
 *  Debug.Log($"<color=yellow></color>");      this is for performance
 *  
 *  Debug.Log($"<color=green></color>");      success
 * 
 *  at thestart you cant have the timer
 * 
 * 
 * 
 * vvvvvvvvvvvvvvvvvvvvv to do vvvvvvvvvvvvvvvvvvvvv
 * 
 *      vvvvvv HIGH PRIORITY vvvvvv
 *          
 *          create rooms from corridors
 *          
 *          MAJOR ISSUE WITH THE SCRIPTABLE OBJ THING AGAIN IT CANT BE IN ANOTHER CLASS OR IT CRASHES
 *          
 *          weigthed tiles spawn for WFC
 *          
 *          when we skip the path gen because of one room the walls dont get generated therefore the fll gen doesnt work
 *          
 *          
 *          
 *      
 *      vvvvvv MID PRIORITY vvvvvv
 *      
 *         for delu do somethign either spanw the points at rnadom and use the triangluation to draw the corridors of use poisson, the workflow should be similar to the L-system
 *         
 *         Look into making the flow better with less time wasted
 *         
 *         Fix the graph thing placement
 *         
 *         Similar to the particle effect have a random between two integers thing
 *         
 *         Add undo button
 *         
 *         Need to move all the scriptable objects into the new class
 *      
 *      
 *      
 *      vvvvvv LOW PRIORITY vvvvvv
 *  
 *          A flood fill algo that fails should mean that a boundary is reached, to look into to redo the wall checker  
 *   
 *          k-group/mean to find regions? but what could regions be needed for? different tileset spawn
 *   
 *          look into different textures for the marching cubes    
 *   
 *          Add gizmos to the scene to show each room detail IDK  
 *   
 *   
 *   
 *   
 *   
 *   
 *   
 *   ALGOS ISSUE:
 * 
 *   GRAPH GRAMMAR  -- mid priority
 *   OTHER PATHFINDINGS   -- high priority       Might drop BFS and Do DFS only
 *   PERLIN WORMS  --  to check    -- mid priority
 *   fix diamond square   -- high priority
 *   
 *   
 *   ALGOS NOTES:
 * 
 *   
 *   
 *   
 *   
 *   KNOWN BUGS:
 *   
 *   still not reaching the end on the bezier curve could be the fact that it chooses a wall
 * 
 */