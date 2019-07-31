Thank you for choosing us !

There are 2 demo scenes in package.
=> "Demo3D" demonstrates all outline features with 3d mesh and animated character.
=> "Demo2D" demonstrates all outline features with 2d sprite renderer.

What is in "Demo3D" ?
=> A scene demonstrates normal expand outline + post process based outline.
   Select the main camera, look the demo component inspector, outline features can be controlled from here.
=> Press on left mouse button and move will rotate the main camera. Press W,S,A,D,Q,E to move camera.
=> Press on right mouse button and move will trigger the selected outline effect.
=> Select gameobject in Hierarchy window, you will see individual outline control parameters in inspector.
=> There is a "Zombunny Transparent" gameobject in Hierarchy window, it demonstrates transparent outline feature.

What is in "Demo2D" ?
=> Demonstrates outline shaders work with 2d sprite renderer.
=> Just move the cursor to the girl sprite, you will see the outline result.
=> There are 2 kinds of sprite outline tech, image based and sdf (signed distance field) based.
   Image based outline is a "legacy" tech, maybe better performance friendly on low-end platform.
   Sdf based outline is what I want to recommend, high quality and much more possibility with an additional sdf texture.

These 2 demo scenes demonstrate all features, please refer it as example usage.

== Note ==========================================================================================================
There is a known compatible issue between different unity version.
It will cause a problem that outline width is super big.
I make some solution try to solve it, however the problem looks like still exist in some unity version.

If you unfortunate meet super big outline width problem.
Solution 1:
  Use extremely small outline width.
  Specifically use very small "_OutlineWidth" shader property.
Solution 2:
  Please open "Outline.cginc".
  Try use line 26 or line 28 shader code.
  This =>     o.pos.xy += offset * o.pos.z * _OutlineWidth * dist;
  Or this =>  o.pos.xy += offset * o.pos.z * _OutlineWidth / dist;
  Find which one is work fine with your unity version.
  
== Note ==========================================================================================================

If you like it, please give us a good review on asset store. We will keep moving !
Any question, suggestion or requesting, please contact qq_d_y@163.com.
Hope we can help more and more unity3d developers.