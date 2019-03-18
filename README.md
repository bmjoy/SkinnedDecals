#SkinnedDecals
A simple un-optimized skinned decal system for Unity.

#How it works
The system tests each vertice of a triangle against the 6 planes of a view frustum (ortho). You can get view frustum planes by using GeometryUtility.CalculateFrustumPlanes().

#Note
The system is not optimized, therefore not actually usable in a real project. The decal creation is costy (around 30ms here), could be threaded, and the decals skinned meshes are not combined (lots of drawcalls).

![skinDecal1.gif]
