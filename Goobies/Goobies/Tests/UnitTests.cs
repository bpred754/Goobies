using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Goobies;
using Microsoft.Xna.Framework;

namespace Goobies
{
    [TestFixture]
    public class UnitTests
    {
        private int team1 = 0;
        private int team2 = 1;

        [Test]
        public void initializationTest()
        {
            Map map = new Map(10, 10);

            Unit circle = new CircleGooby(map, team1, 0, 1);
            Unit square = new SquareGooby(map, team1, 2, 3);
            Unit triangle = new TriangleGooby(map, team1, 4, 5);
            Unit diamond = new DiamondGooby(map, team2, 6, 7);
            Unit cross = new CrossGooby(map, team2, 8, 9);

            Assert.AreEqual(circle.getXPosition(), 0);
            Assert.AreEqual(circle.getYPosition(), 1);
            Assert.AreEqual(square.getXPosition(), 2);
            Assert.AreEqual(square.getYPosition(), 3);
            Assert.AreEqual(triangle.getXPosition(), 4);
            Assert.AreEqual(triangle.getYPosition(), 5);
            Assert.AreEqual(diamond.getXPosition(), 6);
            Assert.AreEqual(diamond.getYPosition(), 7);
            Assert.AreEqual(cross.getXPosition(), 8);
            Assert.AreEqual(cross.getYPosition(), 9);

            // Testing Initialization of Unit constructor
            Assert.AreEqual(circle.getEnergy(), 100);
            Assert.AreEqual(circle.getHealth(), 100);
        }

        [Test]
        public void unitMovementTest()
        {
            /* Test all plain map */
            Map map = makePlainMap();

            Unit square1 = new SquareGooby(map, team1, 0, 0);
            Unit square2 = new SquareGooby(map, team1, 3, 0);
            Unit square3 = new SquareGooby(map, team1, 3, 3);
            Unit square4 = new SquareGooby(map, team1, 0, 3);
            Unit square5 = new SquareGooby(map, team1, 9, 8);

            // ADD TESTS FOR DIFFERENT MOVEMENT COSTS (smaller costs = more movementLocations)
            Assert.AreEqual(5, square1.getMovementLocations(50).Count);
            Assert.AreEqual(8, square2.getMovementLocations(50).Count);
            Assert.AreEqual(12, square3.getMovementLocations(50).Count);
            Assert.AreEqual(8, square4.getMovementLocations(50).Count);
            Assert.AreEqual(7, square5.getMovementLocations(50).Count);

            /* Test Map with Mountains and Hills */
            Map map2 = makeCustomMap();
            map2.printMap();

            Unit circle1 = new CircleGooby(map2, team1, 3, 3);
            Unit circle2 = new CircleGooby(map2, team1, 5, 7);
            Unit circle3 = new CircleGooby(map2, team1, 8, 3);
            Unit circle4 = new CircleGooby(map2, team1, 9, 4);
            Unit circle5 = new CircleGooby(map2, team1, 2, 3);
            Unit circle6 = new CircleGooby(map2, team1, 2, 8);
            Unit circle7 = new CircleGooby(map2, team1, 8, 1);

            Assert.AreEqual(10, circle1.getMovementLocations(50).Count);
            Assert.AreEqual(10, circle2.getMovementLocations(50).Count);
            Assert.AreEqual(9, circle3.getMovementLocations(50).Count);
            Assert.AreEqual(2, circle4.getMovementLocations(50).Count);
            Assert.AreEqual(10, circle5.getMovementLocations(50).Count);

            /* Testing Movement */                                       
            //circle1 valid moves checking
            circle1.move(3, 2, 50);
            Assert.AreEqual(3, circle1.getXPosition());
            Assert.AreEqual(2, circle1.getYPosition());
            circle1.move(2, 2, 50);
            Assert.AreEqual(2, circle1.getXPosition());
            Assert.AreEqual(2, circle1.getYPosition());

            //Border Movement Testing circle6 starts at (2,8)
            circle6.move(2, 9, 50);
            Assert.AreEqual(2, circle6.getXPosition());
            Assert.AreEqual(9, circle6.getYPosition());
            //Border Movement Testing circle7 starts at (8,1)
            circle7.move(8, 0, 50);
            Assert.AreEqual(8, circle7.getXPosition());
            Assert.AreEqual(0, circle7.getYPosition());
           
            //Circle 2 moves checking starts at (5,7)
            circle2.move(6, 7, 10);
            Assert.AreEqual(6, circle2.getXPosition());
            Assert.AreEqual(7, circle2.getYPosition());
            circle2.move(6, 8, 10);
            Assert.AreEqual(6, circle2.getXPosition());
            Assert.AreEqual(8, circle2.getYPosition());
            circle2.move(6, 9, 10);
            Assert.AreEqual(6, circle2.getXPosition());
            Assert.AreEqual(9, circle2.getYPosition());
            circle2.move(5, 9, 10);
            Assert.AreEqual(5, circle2.getXPosition());
            Assert.AreEqual(9, circle2.getYPosition());
            circle2.move(5, 8, 10);
            Assert.AreEqual(5, circle2.getXPosition());
            Assert.AreEqual(8, circle2.getYPosition());

            //Testing occupancy; movement; team.
            Unit circle8 = new CircleGooby(map2, team1, 0, 9);
            circle8.move(0, 8, 50);
            circle8.move(0, 7, 50);
            Assert.AreEqual(team1, map2.get(0,9).getTeam());
            Assert.AreEqual(team1, map2.get(0, 8).getTeam());
            Assert.AreEqual(team1, map2.get(0, 7).getTeam());
            Assert.AreEqual(false, map2.get(0, 9).getOccupation());
            Assert.AreEqual(false, map2.get(0, 8).getOccupation());
            Assert.AreEqual(true, map2.get(0, 7).getOccupation()); 
     
            // Testing movement with taking movement cost into consideration
            Unit circle9 = new CircleGooby(map2, team1, 0, 0);
            circle9.move(1,0,30);
            circle9.move(2, 0, 30);
            circle9.move(2, 1, 30);
            circle9.move(2, 2, 30);
            Assert.AreEqual(2, circle9.getXPosition());
            Assert.AreEqual(1, circle9.getYPosition());

            // unit cannot move onto territory with another unit test
            Map map3 = makePlainMap();

            Unit c1 = new CircleGooby(map3, team1, 0,0);
            Unit s1 = new SquareGooby(map3, team1,1,0);

            c1.move(1,0,10);
            Assert.AreEqual(c1.getXPosition(),0);
            Assert.AreEqual(c1.getYPosition(), 0);
        }        

        [Test]
        public void cardinalMovementTest()
        {
            Map map = makeCustomMap();

            Unit square1 = new SquareGooby(map, team1, 1, 1);
            Unit square2 = new SquareGooby(map, team1, 2, 1);
            Unit square3 = new SquareGooby(map, team1, 4, 8);
            Unit square4 = new SquareGooby(map, team1, 6, 8);

            square1.getMovementLocations(50);
            square2.getMovementLocations(50);
            square3.getMovementLocations(50);
            square4.getMovementLocations(20);

            Assert.AreEqual(3, square1.getCardinalLocations().Count);
            Assert.AreEqual(3, square2.getCardinalLocations().Count);
            Assert.AreEqual(3, square3.getCardinalLocations().Count);
            Assert.AreEqual(3, square4.getCardinalLocations().Count);
        }

        [Test]
        public void squareAttackTest()
        {
            /* Test All Plain Map */
            Map map = makePlainMap();

            Unit square1 = new SquareGooby(map, team1, 2, 5);
            Unit square2 = new SquareGooby(map, team1, 5, 2);
            Unit square3 = new SquareGooby(map, team1, 0, 0);
            Unit square4 = new CircleGooby(map, team1, 0, 1);

            Unit circle1 = new CircleGooby(map, team2, 3, 5);
            Unit circle2 = new CircleGooby(map, team2, 6, 3);
            Unit circle3 = new CircleGooby(map, team2, 1, 1);
            Unit circle4 = new CircleGooby(map, team2, 1, 0);

            // Testing Attack Locations List
            Assert.AreEqual(1, square1.getAttackLocations().Count);
            Assert.AreEqual(0, square2.getAttackLocations().Count);
            Assert.AreEqual(1, square3.getAttackLocations().Count);
            Assert.IsTrue(square3.getAttackLocations().Contains(new Vector2(1,0)));
            Assert.IsTrue(square1.getAttackLocations().Contains(new Vector2(3, 5)));

            // Attack Tests
            square1.attack(3, 5); // Attack circle to right
            Assert.AreEqual(0, circle1.getHealth());
            Assert.AreEqual(70, square1.getEnergy());
            square1.attack(2, 4); // Attack empty territory downwards
            Assert.AreEqual(70, square1.getEnergy());

            square3.attack(0, 1); // Attack team upwards
            Assert.AreEqual(100, square4.getHealth());
            Assert.AreEqual(100, square3.getEnergy());
            square3.attack(1, 1); // Attack enemy out of range
            Assert.AreEqual(100, circle3.getHealth());
            Assert.AreEqual(100, square3.getEnergy());

            // Move into attack position and then attack
            square2.move(5, 3, 15);
            square2.attack(6, 3);
            Assert.AreEqual(0, circle2.getHealth());
            Assert.AreEqual(55, square2.getEnergy());
        }

        [Test]
        public void circleAttackTest()
        {
            /* Test All Plain Map */
            Map map = makePlainMap();

            Unit circle1 = new CircleGooby(map, team1, 4, 4);
            Unit circle2 = new CircleGooby(map, team1, 9, 9);
            Unit circle3 = new CircleGooby(map, team1, 1, 5);

            Unit square1 = new CircleGooby(map, team2, 0, 4);
            Unit square2 = new CircleGooby(map, team2, 2, 5);
            Unit square3 = new CircleGooby(map, team2, 6, 6);
            Unit square4 = new CircleGooby(map, team2, 4, 3);
            Unit square5 = new CircleGooby(map, team2, 4, 1);
            Unit square6 = new CircleGooby(map, team2, 7, 7);
            Unit square7 = new CircleGooby(map, team2, 2, 1);

            Assert.AreEqual(true, map.get(4, 1).getOccupation());

            //Testing attackLocations
            Assert.AreEqual(circle1.getAttackLocations().Count,4);
            Assert.AreEqual(circle2.getAttackLocations().Count, 1);
            Assert.AreEqual(circle3.getAttackLocations().Count, 2);

            // Test Attack
            circle1.attack(4, 3); // Attack square below
            Assert.AreEqual(50, square4.getHealth());
            Assert.AreEqual(50, circle1.getEnergy());
            circle1.attack(6, 6); // Attack square on edge of attack perimeter
            Assert.AreEqual(50, square3.getHealth());
            Assert.AreEqual(0, circle1.getEnergy());

            // Test Attack out of range

            for (int i = 0; i < circle2.getAttackLocations().Count; i++)
            {
                Console.WriteLine(""+ circle2.getAttackLocations().ElementAt(i));
            }

                circle2.attack(6, 6);
            //Assert.AreEqual(50, square3.getHealth());
            Assert.AreEqual(100, circle2.getEnergy());

            // Test move and then attack
            circle2.move(9,8,10);
            circle2.move(9, 7, 10);
            Assert.AreEqual(80, circle2.getEnergy());
            circle2.attack(6, 6);
            Assert.AreEqual(0, square3.getHealth());
            Assert.AreEqual(30, circle2.getEnergy());

            /* Advanced Testing */
            // Cannot shoot through unit test
            Map map2 = makePlainMap();

            Unit c1 = new CircleGooby(map2, team1, 4, 4);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Unit square = new SquareGooby(map2, team2, i, j);
                }
            }
            Assert.AreEqual(3,c1.getAttackLocations().Count);

            // Unit cannot shoot through mountain test
            map2.get(4, 5).setElevation(elevation.mountain);
            Unit s1 = new SquareGooby(map2, team2, 4, 6);
            c1.attack(4, 6);
            Assert.AreEqual(100, s1.getHealth());
            Assert.AreEqual(100, c1.getEnergy());

            // Cannot shoot unit on mountain range when not on ridge test
            map2.get(4, 6).setElevation(elevation.mountain);
            c1.attack(4,6);
            Assert.AreEqual(100, s1.getHealth());
            Assert.AreEqual(100, c1.getEnergy());

            // Can shoot unit on a mountain when also on a mountain
            map2.get(4, 4).setElevation(elevation.mountain);
            c1.attack(4, 6);
            Assert.AreEqual(50, s1.getHealth());
            Assert.AreEqual(50, c1.getEnergy());

            // From on a mountain, unit can shoot enemy that is on a different mountain with plains in between
            map2.get(4, 5).setElevation(elevation.mountain);
            c1.attack(4, 6);
            Assert.AreEqual(0, s1.getHealth());
            Assert.AreEqual(0, c1.getEnergy());
        }

        [Test]
        public void triangleAttackTest()
        {
            /* Test All Plain Map */
            Map map = makePlainMap();

            Unit triangle1 = new TriangleGooby(map, team1, 4,4);

            Unit square1 = new TriangleGooby(map, team2, 4, 2);
            Unit square2 = new TriangleGooby(map, team2, 4, 5);
            Unit square3 = new TriangleGooby(map, team2, 8, 4);
            Unit square4 = new TriangleGooby(map, team2, 1, 4);
            Unit square5 = new TriangleGooby(map, team2, 2, 6);
            Unit square6 = new TriangleGooby(map, team2, 7, 7);
            Unit square7 = new TriangleGooby(map, team2, 5, 3);
            Unit square8 = new TriangleGooby(map, team2, 2, 2);
            Unit square9 = new TriangleGooby(map, team2, 0, 7);
            Unit square10 = new TriangleGooby(map, team2, 3, 1);
            Unit square11 = new TriangleGooby(map, team2, 3, 0);
            
            // Testing Attack Locations
            Assert.AreEqual(triangle1.getAttackLocations().Count, 8);

            // Attack Tests
            triangle1.attack(4,5); // square2
            Assert.AreEqual(0, square2.getHealth());
            Assert.AreEqual(0, triangle1.getEnergy());
            triangle1.setEnergy(100); // Reset energy
            Assert.AreEqual(100, triangle1.getEnergy());
            triangle1.attack(0, 7); // square9 out of range
            Assert.AreEqual(100, square9.getHealth());
            Assert.AreEqual(100, triangle1.getEnergy());
            
            // Move and then attack test
            triangle1.move(3,4,10);
            Assert.AreEqual(triangle1.getXPosition(),3);
            Assert.AreEqual(triangle1.getYPosition(), 4);
            Assert.AreEqual(5,triangle1.getAttackLocations().Count);
            triangle1.setEnergy(100);
            triangle1.attack(0,7);
            Assert.AreEqual(0, square9.getHealth());
            Assert.AreEqual(0, triangle1.getEnergy());

            // Cannot shoot through unit test
            triangle1.setEnergy(100);
            triangle1.attack(3, 0);
            Assert.AreEqual(100, square11.getHealth());
            Assert.AreEqual(100, triangle1.getEnergy());

            // Cannot shoot through Mountain test
            map.get(7, 4).setElevation(elevation.mountain);
            triangle1.setEnergy(100);
            triangle1.attack(8, 4);
            Assert.AreEqual(100, square3.getHealth());
            Assert.AreEqual(100, triangle1.getEnergy());

            // Cannot shoot unit on mountain range when not on ridge test
            map.get(8, 4).setElevation(elevation.mountain);
            triangle1.setEnergy(100);
            triangle1.attack(8, 4);
            Assert.AreEqual(100, square3.getHealth());
            Assert.AreEqual(100, triangle1.getEnergy());

            // Can shoot unit on a mountain when also on a mountain
            map.get(3, 4).setElevation(elevation.mountain);
            triangle1.setEnergy(100);
            triangle1.attack(8,4);
            Assert.AreEqual(0, square3.getHealth());
            Assert.AreEqual(0, triangle1.getEnergy());
        }

        [Test]
        public void diamondAttackTest()
        {
            /* Test All Plain Map */
            Map map = makePlainMap();

            Unit diamond1 = new DiamondGooby(map,team1,4,4);

            Unit circle1 = new CircleGooby(map, team2, 4, 3);
            Unit circle2 = new CircleGooby(map, team2, 2, 7);
            Unit circle3 = new CircleGooby(map, team2, 4, 2);
            Unit circle4 = new CircleGooby(map, team2, 8, 4);
            Unit circle5 = new CircleGooby(map, team2, 1, 4);
            Unit circle6 = new CircleGooby(map, team2, 7, 7);
            Unit circle7 = new CircleGooby(map, team2, 0, 0);

            // Testing Attack Locations
            Assert.AreEqual(diamond1.getAttackLocations().Count, 5);

            // Attack tests
            diamond1.attack(1, 4); // Attack circle5
            Assert.AreEqual(50, circle5.getHealth());
            Assert.AreEqual(0, diamond1.getEnergy());
            diamond1.setEnergy(100);
            diamond1.attack(4, 2); // Attack 2 goobies next to each other
            Assert.AreEqual(50, circle3.getHealth());
            Assert.AreEqual(50, circle1.getHealth());
            Assert.AreEqual(0, diamond1.getEnergy());

            // Move and then attack test
            diamond1.setEnergy(200);
            diamond1.move(5, 4,100);
            diamond1.attack(7, 7);
            Assert.AreEqual(50, circle6.getHealth());
            Assert.AreEqual(0, diamond1.getEnergy());
        }

        [Test]
        public void mountainAttackRangeTest()
        {
            /* Test Custom Map */
            Map map = makeCustomMap();

            Unit circle1 = new CircleGooby(map, team1, 2, 2);
            Unit circle2 = new CircleGooby(map, team2, 1, 5);
            Unit circle3 = new CircleGooby(map, team2, 2, 7);
            Unit circle4 = new CircleGooby(map, team2, 3, 7);
            Unit circle5 = new CircleGooby(map, team1, 5, 7);

            //On Creation
            Assert.AreEqual(5, circle1.getAttackRange());
            Assert.AreEqual(2,circle1.getAttackLocations().Count);

            //After Moving off of Mountain
            circle1.move(1, 2, 10);
            Assert.AreEqual(4, circle1.getAttackRange());
            Assert.AreEqual(1, circle1.getAttackLocations().Count);

            //After Moving
            circle1.move(2, 2, 10);
            Assert.AreEqual(5, circle1.getAttackRange());
            Assert.AreEqual(4, circle2.getAttackRange());
            Assert.AreEqual(2, circle1.getAttackLocations().Count);
        }

        public Map makePlainMap()
        {
            Territory[,] mapGrid = new Territory[10, 10];

            // Create map with all plains
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    mapGrid[i, j] = new Territory(i, j);
                }
            }
            Map map = new Map(mapGrid);

            return map;
        }

        public Map makeCustomMap()
        {
            Territory[,] mapGrid = new Territory[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    mapGrid[i, j] = new Territory(i, j);
                }
            }

            mapGrid[2, 1].setElevation(elevation.hill);
            mapGrid[2, 3].setElevation(elevation.hill);
            mapGrid[1, 2].setElevation(elevation.hill);
            mapGrid[3, 2].setElevation(elevation.hill);
            mapGrid[9, 3].setElevation(elevation.hill);
            mapGrid[5, 9].setElevation(elevation.hill);

            mapGrid[2, 2].setElevation(elevation.mountain);
            mapGrid[5, 8].setElevation(elevation.mountain);
            mapGrid[9, 4].setElevation(elevation.mountain);

            Map map = new Map(mapGrid);

            return map;
        }
    }
}

