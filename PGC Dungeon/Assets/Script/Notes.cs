
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
 *          wieghts WFC
 * 
 *         issues with the chunk   --- fixed i think
 *                 
 *                 
 *                 
 *      vvvvvv MID PRIORITY vvvvvv
 *      
 *         Fix the graph thing placement
 *         
 *         Similar to the particle effect have a random between two integers thing
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
 *          check on empty strings when loading stuff can be an easy function relatin back to the pcgManager
 *   
 *   
 *   
 *   
 *   ALGOS ISSUE:
 * 
 *   GRAPH GRAMMAR  -- mid priority    -- new feature lock
 *   OTHER PATHFINDINGS   -- high priority       Might drop BFS and Do DFS only
 *   PERLIN WORMS  --  to check    -- mid priority
 *   
 *   
 *   
 *   
 *  ALGOS NOTES:
 *  
 *  voronoi -- done 
 *  random Walk -- done
 *  CA   -- done
 *  ran room gen --- should be done
 *  Lsystem   -- need to add the room gen from corr
 *  delu   -- to check but should be done
 *  wfc  ---             done
 *  perlin noise  --  done
 *  perlin worm   -- prob to scrap
 *  diamondSquare  ---   done    
 *  other -- done
 *  
 *  POISSANT IS DONE
 *  
 *  
 *  this might have issue with the chunk and how we are actually going to generate everythin
 *  loader -- the player loads the map, this map should have its corridor set as corridors and room as room
 *  the loader is used to populate the envi.
 *  if i have all of the rooms i can show them in a list format of sorts and have them saved in a class this class holst the room, the middle where the gizmos is going to spawn and a select thign so 
 *  
 *  its going to have a button asking for    small stuff like pebbles
 *                                           bigger stuff like pillars
 *                                           
 *  button telling to generate that stuff in the selected rooms   //
 *  
 *  need to do the dynamic chunk rendering stuff
 *   
 *  maybe add the randombetween  100% can be a function
 * 
 * 
 * delete the refresh plane button if its the same as the refresh main algo comp buitton
 * 
 * might have issues with the mesh gen when it too big
 * 
 * https://www.pixilart.com/draw/64x64-6452daa78a#
 * 
 * redo the order of the pcgmanager scirpt
 */