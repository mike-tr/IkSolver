First of all thanks to this repo:

https://github.com/ditzel/SimpleIK
and this video: https://www.youtube.com/watch?v=qqOAzn05fvk

from there i got the basis for my redo at the same task, at first i did the 3DIkSolver
as in the video above with few changes mainly to get better performance and i got it to work 5 times as fast (per 10k simulations)
from 300ms, with 4 chainLength to, about ~80 at any given time.


next step was to implement the same thing for 2D, but with few rules, 
1. it build especially for hinge joints 2D, with means the "real" position is the anchor and not the transform.position
2. it should respect the physic engine, A.k.a gravity and any forces applied to the body must stay perserved.

so calculating the desireble anchor positions is actually easy, its straight up the 3D code, but for Vectors2D.
as for how to make the anchors go to the position, i desided, to go with both.
AddForce, and Torque.
AddForce is great to quicly move the desireble part to the position, while also mentaining stability.
add torque has much less stability (a.k.a jiggering) but, it provide much more ( resistance ) to forces.
A.k.a if you try to push from the ground, add force would result in very weak push while near the end target.
but add torque would always support a desireble rotation rather then position and thus eliminate that factor.
thus making pushing from ground, a real possibility.

as to how we keep the forces at the right ammount? we simply preserve it, we  get the "root" of the System, and thus we simply,pass
unneeded velocity from the system into the root, so if the system tries to get from point A to B, and gravity is interacting, 
the unneeded gravity force would simply transfer unto the main body, as with "real" life objects.
