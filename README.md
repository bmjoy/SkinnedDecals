# SkinnedDecals
A simple un-optimized skinned decal system for Unity.

# How it works
The system tests each vertice of a triangle against the 6 planes of a view frustum (ortho) using GeometryUtility.CalculateFrustumPlanes().
