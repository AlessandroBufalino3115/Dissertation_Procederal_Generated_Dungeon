
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
 *          weigthed tiles spawn for WFC
 *            -texture spawning
 *          
 *          when we skip the path gen because of one room the walls dont get generated therefore the fll gen doesnt work   -- this seems to work
 *          -- the issue will be with the undo when the rooms are deleted could do a sort of que or we techincally know the index
 *          
 *          
 *          
 *          issues with the chunk   --- fixed i think
 *                    
 *          fix diamond sqaure
 *          
 *       
 *          if it recognises them as corridors i can actually load it back in and get the corridors back
 *    
 *          
 *          
 *          on drawing 
 *          
 *      
 *          
 *      
 *      vvvvvv MID PRIORITY vvvvvv
 *      
 *          full bezier cave system as solo algo basically center bit as the arena and then loads of point around delu trig aroudn them   -- done
 *      
 *         for delu do somethign either spanw the points at rnadom and use the triangluation to draw the corridors of use poisson, the workflow should be similar to the L-system   
 *         
 *         Look into making the flow better with less time wasted
 *         
 *         Fix the graph thing placement
 *         
 *         Similar to the particle effect have a random between two integers thing
 *         
 *         Add undo button   -- done
 *         
 *         Need to move all the scriptable objects into the new class   -- dropped
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
 *          check on empty strings when loading stuff can be an easy function relatin back to the pcgManager
 *   
 *   
 *   
 *   
 *   ALGOS ISSUE:
 * 
 *   GRAPH GRAMMAR  -- mid priority    -- new feautre lock
 *   OTHER PATHFINDINGS   -- high priority       Might drop BFS and Do DFS only
 *   PERLIN WORMS  --  to check    -- mid priority
 *   fix diamond square   -- high priority
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
 *  delu   -- need to add the localised random (maybe)      -- when i will try and get all the corrdior this might be a problem
 *  wfc  ---              texture recognition is there just needs to buidl the rule
 *  perlin noise  --  done
 *  perlin worm   -- prob to scrap
 *  diamondSquare  ---   done    just fix the algo apart from than done         
 *  
 *  
 *  this might have issue with the chunk and how we are actually going to generate everythin
 *  loader -- the player loads the map, this map should have its corridor set as corridors and room as room
 *  the loader is used to populate the envi.
 *  if i have all of the rooms i can show them in a list format of sorts and have them saved in a class this class holst the room, the middle where the gizmos is going to spawn and a select thign so 
 *  
 *  its going to ahve a button asking for    small stuff like pebbles
 *                                           bigger stuff like pillars
 *                                           
 *  button telling to generate that stuff in the selected rooms
 *  
 *  need to do the dynamic chunk rendering stuff
 *   
 *  maybe add the randombetween  100% can be a function
 * 
 * 
 * there is anew algo to implement whic si already done
 * poissant might need to wait 
 * 
 * 
 * the thing that takes priority right now is the wfc and the diamon
 * delte the refresh plane button if its the same as the refresh main algo comp buitton
 * 
 * https://www.pixilart.com/draw/64x64-6452daa78a#
 */