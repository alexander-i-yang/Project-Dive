using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Phys {
    public abstract class Solid : PhysObj {
        public override bool MoveGeneral(Vector2 direction, int magnitude, Func<PhysObj, Vector2, bool> onCollide) {
            if (magnitude < 0) throw new ArgumentException("Magnitude must be >0");

            int remainder = magnitude;

            Actor[] allActors = PhysObj.GetActors();
            
            // If the actor moves at least 1 pixel, Move one pixel at a time
            while (remainder > 0) {
                if (CheckCollisions(Vector2.zero, (p, d) => {
                    return p != this && onCollide(p, d);
                })) {
                    Debug.LogError("Stuck");
                }

                List<Actor> ridingActors = GetRidingActors(allActors);
                bool collision = CheckCollisions(direction, (p, d) => {
                    if (p == this) {
                        return false;
                    }
                    
                    if (ridingActors.Contains(p)) {
                        ridingActors.Remove((Actor)p);
                    }
                    
                    if (!allActors.Contains(p)) {
                        if (onCollide(p, d)) {
                            return true;
                        }
                    } else {
                        p.MoveGeneral(direction, 1, p.Squish);
                    }

                    return false;
                });

                foreach (var a in ridingActors) {
                    a.Move(direction);
                }
                if (collision) return true;
                
                transform.position += new Vector3((int)direction.x, (int)direction.y, 0);
                nextFrameOffset += direction;
                remainder -= 1;
            }
            
            return false;
        }

        public List<Actor> GetRidingActors(Actor[] allActors) {
            return allActors.Where(c => c.IsRiding(this)).ToList();
        }

        public override bool Squish(PhysObj p, Vector2 d) {
            return false;
        }
    }
}