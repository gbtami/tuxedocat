﻿/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2016 Nathan McCrina
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */

using System;

namespace TuxedoCat
{
    public class MoveGenerator
    {
        private static ulong[] FileMask;
        private static ulong[] RankMask;
        private static ulong[] SWNEMask;
        private static ulong[] NWSEMask;

        private static ulong[] KnightAttacks;
        private static ulong[] KingAttacks;
        private static ulong[] RayAttacksN;
        private static ulong[] RayAttacksS;
        private static ulong[] RayAttacksE;
        private static ulong[] RayAttacksW;
        private static ulong[] RayAttacksNE;
        private static ulong[] RayAttacksNW;
        private static ulong[] RayAttacksSE;
        private static ulong[] RayAttacksSW;

        private static TuxedoCatUtility util;
        private MoveList moveList;

        public MoveGenerator()
        {
            InitializeFileMask();
            InitializeRankMask();
            InitializeNWSEMask();
            InitializeSWNEMask();
            InitializeKnightAttacks();
            InitializeKingAttacks();
            InitializeRayAttacksN();
            InitializeRayAttacksS();
            InitializeRayAttacksE();
            InitializeRayAttacksW();
            InitializeRayAttacksNE();
            InitializeRayAttacksNW();
            InitializeRayAttacksSE();
            InitializeRayAttacksSW();

            moveList = new MoveList();
            util = new TuxedoCatUtility();
        }

        public Move[] GenerateMoves(ref Position position)
        {
            ulong pieces = position.ColorToMove == PieceColor.WHITE ? position.WhitePieces : position.BlackPieces;
            ulong currentPiece;
            PieceRank rank;
            bool inCheck = false;

            moveList.clear();

            if (IsSquareAttacked(position.ColorToMove ==
                PieceColor.WHITE ? position.WhiteKing : position.BlackKing, ref position))
            {
                inCheck = true;
            }

            while (pieces != 0x0000000000000000UL)
            {
                currentPiece = 0x0000000000000001UL << util.GetLSB(pieces);
                rank = position.GetRankAt(currentPiece).Value;

                if (rank == PieceRank.KNIGHT)
                {
                    if (!IsPiecePinnedFile(currentPiece, ref position)
                        && !IsPiecePinnedRank(currentPiece, ref position)
                        && !IsPiecePinnedNWSE(currentPiece, ref position)
                        && !IsPiecePinnedSWNE(currentPiece, ref position))
                    {
                        GenerateKnightMovesAt(currentPiece, ref position, inCheck);
                    }
                }
                else if (rank == PieceRank.BISHOP)
                {
                    if (!IsPiecePinnedFile(currentPiece, ref position)
                        && !IsPiecePinnedRank(currentPiece, ref position))
                    {
                        if (!IsPiecePinnedNWSE(currentPiece, ref position))
                        {
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x02, inCheck);
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x20, inCheck);
                        }

                        if (!IsPiecePinnedSWNE(currentPiece, ref position))
                        {
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x08, inCheck);
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x80, inCheck);
                        }
                    }
                }
                else if (rank == PieceRank.ROOK)
                {
                    if (!IsPiecePinnedNWSE(currentPiece, ref position)
                        && !IsPiecePinnedSWNE(currentPiece, ref position))
                    {
                        if (!IsPiecePinnedRank(currentPiece, ref position))
                        {
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x01, inCheck);
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x10, inCheck);
                        }

                        if (!IsPiecePinnedFile(currentPiece, ref position))
                        {
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x04, inCheck);
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x40, inCheck);
                        }
                    }
                }
                else if (rank == PieceRank.QUEEN)
                {
                    if (!IsPiecePinnedNWSE(currentPiece, ref position)
                        && !IsPiecePinnedSWNE(currentPiece, ref position))
                    {
                        if (!IsPiecePinnedRank(currentPiece, ref position))
                        {
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x01, inCheck);
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x10, inCheck);
                        }

                        if (!IsPiecePinnedFile(currentPiece, ref position))
                        {
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x04, inCheck);
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x40, inCheck);
                        }
                    }

                    if (!IsPiecePinnedFile(currentPiece, ref position)
                        && !IsPiecePinnedRank(currentPiece, ref position))
                    {
                        if (!IsPiecePinnedNWSE(currentPiece, ref position))
                        {
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x02, inCheck);
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x20, inCheck);
                        }

                        if (!IsPiecePinnedSWNE(currentPiece, ref position))
                        {
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x08, inCheck);
                            GenerateSlidingMovesAt(currentPiece, ref position, 0x80, inCheck);
                        }
                    }
                }
                else if (rank == PieceRank.KING)
                {
                    GenerateKingMovesAt(currentPiece, ref position, inCheck);
                }
                else if (rank == PieceRank.PAWN)
                {
                    GeneratePawnMovesAt(currentPiece, ref position, inCheck);
                }

                pieces = pieces & ~currentPiece;
            }

            return moveList.getSlice(moveList.Count);
        }

        private bool IsPiecePinnedNWSE(ulong location, ref Position position)
        {
            return IsPiecePinned(location, ref position, 7);
        }

        private bool IsPiecePinnedSWNE(ulong location, ref Position position)
        {
            return IsPiecePinned(location, ref position, 9);
        }

        private bool IsPiecePinnedRank(ulong location, ref Position position)
        {
            return IsPiecePinned(location, ref position, 1);
        }

        private bool IsPiecePinnedFile(ulong location, ref Position position)
        {
            return IsPiecePinned(location, ref position, 8);
        }

        private void InitializeFileMask()
        {
            FileMask = new ulong[64];
            ulong currentMask;
            ulong currentSquare = 0x0000000000000001UL;

            for (int index = 0; index < 64; index++)
            {
                currentMask = 0x0101010101010101UL;

                for (int shiftIndex = 0; shiftIndex < 8; shiftIndex++)
                {
                    if ((currentSquare & currentMask) != 0x0000000000000000UL)
                    {
                        FileMask[index] = currentMask;
                        break;
                    }

                    currentMask = currentMask << 1;
                }

                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRankMask()
        {
            RankMask = new ulong[64];
            ulong currentMask;
            ulong currentSquare = 0x0000000000000001UL;

            for (int index = 0; index < 64; index++)
            {
                currentMask = 0x00000000000000FFUL;

                for (int shiftIndex = 0; shiftIndex < 8; shiftIndex++)
                {
                    if ((currentSquare & currentMask) != 0x0000000000000000UL)
                    {
                        RankMask[index] = currentMask;
                        break;
                    }

                    currentMask = currentMask << 8;
                }

                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeSWNEMask()
        {
            SWNEMask = new ulong[64];
            ulong currentMask;
            ulong currentSquare = 0x0000000000000001UL;

            for (int index = 0; index < 64; index++)
            {
                currentMask = 0x0000000000000080UL;

                for (int shiftIndex = 0; shiftIndex < 16; shiftIndex++)
                {
                    if ((currentSquare & currentMask) != 0x0000000000000000UL)
                    {
                        SWNEMask[index] = currentMask;
                        break;
                    }

                    currentMask = currentMask << 8;

                    if (shiftIndex < 7)
                    {
                        currentMask = currentMask | ((0x0000000000000001UL) << (6 - shiftIndex));
                    }
                }

                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeNWSEMask()
        {
            NWSEMask = new ulong[64];
            ulong currentMask;
            ulong currentSquare = 0x0000000000000001UL;

            for (int index = 0; index < 64; index++)
            {
                currentMask = 0x0000000000000001UL;

                for (int shiftIndex = 0; shiftIndex < 16; shiftIndex++)
                {
                    if ((currentSquare & currentMask) != 0x0000000000000000UL)
                    {
                        NWSEMask[index] = currentMask;
                        break;
                    }

                    currentMask = currentMask << 8;

                    if (shiftIndex < 7)
                    {
                        currentMask = currentMask | ((0x0000000000000001UL) << (shiftIndex + 1));
                    }
                }

                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeKnightAttacks()
        {
            KnightAttacks = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                moves = moves | ((currentSquare & 0x0000FEFEFEFEFEFEUL) << 15);
                moves = moves | ((currentSquare & 0x00007F7F7F7F7F7FUL) << 17);
                moves = moves | ((currentSquare & 0x003F3F3F3F3F3F3FUL) << 10);
                moves = moves | ((currentSquare & 0x3F3F3F3F3F3F3F00UL) >> 6);
                moves = moves | ((currentSquare & 0x7F7F7F7F7F7F0000UL) >> 15);
                moves = moves | ((currentSquare & 0xFEFEFEFEFEFE0000UL) >> 17);
                moves = moves | ((currentSquare & 0xFCFCFCFCFCFCFC00UL) >> 10);
                moves = moves | ((currentSquare & 0x00FCFCFCFCFCFCFCUL) << 6);

                KnightAttacks[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeKingAttacks()
        {
            KingAttacks = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                moves = moves | ((currentSquare & 0x00FFFFFFFFFFFFFFUL) << 8);
                moves = moves | ((currentSquare & 0x007F7F7F7F7F7F7FUL) << 9);
                moves = moves | ((currentSquare & 0x7F7F7F7F7F7F7F7FUL) << 1);
                moves = moves | ((currentSquare & 0x7F7F7F7F7F7F7F7FUL) >> 7);
                moves = moves | ((currentSquare & 0xFFFFFFFFFFFFFF00UL) >> 8);
                moves = moves | ((currentSquare & 0xFEFEFEFEFEFEFE00UL) >> 9);
                moves = moves | ((currentSquare & 0xFEFEFEFEFEFEFEFEUL) >> 1);
                moves = moves | ((currentSquare & 0x00FEFEFEFEFEFEFEUL) << 7);

                KingAttacks[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRayAttacksN()
        {
            RayAttacksN = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong tmpSquare;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                tmpSquare = currentSquare;

                while ((tmpSquare & 0x00FFFFFFFFFFFFFFUL) != 0x0000000000000000UL)
                {
                    tmpSquare = tmpSquare << 8;
                    moves = moves | tmpSquare;
                }

                RayAttacksN[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRayAttacksS()
        {
            RayAttacksS = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong tmpSquare;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                tmpSquare = currentSquare;

                while ((tmpSquare & 0xFFFFFFFFFFFFFF00UL) != 0x0000000000000000UL)
                {
                    tmpSquare = tmpSquare >> 8;
                    moves = moves | tmpSquare;
                }

                RayAttacksS[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRayAttacksE()
        {
            RayAttacksE = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong tmpSquare;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                tmpSquare = currentSquare;

                while ((tmpSquare & 0x7F7F7F7F7F7F7F7FUL) != 0x0000000000000000UL)
                {
                    tmpSquare = tmpSquare << 1;
                    moves = moves | tmpSquare;
                }

                RayAttacksE[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRayAttacksW()
        {
            RayAttacksW = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong tmpSquare;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                tmpSquare = currentSquare;

                while ((tmpSquare & 0xFEFEFEFEFEFEFEFEUL) != 0x0000000000000000UL)
                {
                    tmpSquare = tmpSquare >> 1;
                    moves = moves | tmpSquare;
                }

                RayAttacksW[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRayAttacksNE()
        {
            RayAttacksNE = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong tmpSquare;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                tmpSquare = currentSquare;

                while ((tmpSquare & 0x007F7F7F7F7F7F7FUL) != 0x0000000000000000UL)
                {
                    tmpSquare = tmpSquare << 9;
                    moves = moves | tmpSquare;
                }

                RayAttacksNE[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRayAttacksNW()
        {
            RayAttacksNW = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong tmpSquare;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                tmpSquare = currentSquare;

                while ((tmpSquare & 0x00FEFEFEFEFEFEFEUL) != 0x0000000000000000UL)
                {
                    tmpSquare = tmpSquare << 7;
                    moves = moves | tmpSquare;
                }

                RayAttacksNW[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRayAttacksSE()
        {
            RayAttacksSE = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong tmpSquare;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                tmpSquare = currentSquare;

                while ((tmpSquare & 0x7F7F7F7F7F7F7F00UL) != 0x0000000000000000UL)
                {
                    tmpSquare = tmpSquare >> 7;
                    moves = moves | tmpSquare;
                }

                RayAttacksSE[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private void InitializeRayAttacksSW()
        {
            RayAttacksSW = new ulong[64];
            ulong currentSquare = 0x0000000000000001UL;
            ulong tmpSquare;
            ulong moves;

            for (int i = 0; i < 64; i++)
            {
                moves = 0x0000000000000000UL;
                tmpSquare = currentSquare;

                while ((tmpSquare & 0xFEFEFEFEFEFEFE00UL) != 0x0000000000000000UL)
                {
                    tmpSquare = tmpSquare >> 9;
                    moves = moves | tmpSquare;
                }

                RayAttacksSW[i] = moves;
                currentSquare = currentSquare << 1;
            }
        }

        private bool IsPiecePinned(ulong location, ref Position position, int offset)
        {
            bool result = true;
            int locationMaskIndex;
            PieceColor pinningColor =
                    position.GetColorAt(location) == PieceColor.WHITE ? PieceColor.BLACK : PieceColor.WHITE;
            ulong pinMask;
            ulong pinnedKingLocation;
            ulong mask;
            ulong occupancy;
            ulong tmpLocation;
            ulong guard;
            bool goUp;

            if (util.PopCount(location) == 1)
            {
                if (pinningColor == PieceColor.WHITE)
                {
                    if (offset == 8 || offset == 1)
                    {
                        pinMask = position.WhiteQueens | position.WhiteRooks;
                    }
                    else
                    {
                        pinMask = position.WhiteQueens | position.WhiteBishops;
                    }

                    pinnedKingLocation = position.BlackKing;
                }
                else
                {
                    if (offset == 8 || offset == 1)
                    {
                        pinMask = position.BlackQueens | position.BlackRooks;
                    }
                    else
                    {
                        pinMask = position.BlackQueens | position.BlackBishops;
                    }

                    pinnedKingLocation = position.WhiteKing;
                }

                locationMaskIndex = util.GetLSB(location);

                if (offset == 8)
                {
                    mask = FileMask[locationMaskIndex];
                }
                else if (offset == 1)
                {
                    mask = RankMask[locationMaskIndex];
                }
                else if (offset == 9)
                {
                    mask = SWNEMask[locationMaskIndex];
                }
                else if (offset == 7)
                {
                    mask = NWSEMask[locationMaskIndex];
                }
                else
                {
                    mask = 0x0000000000000000UL;
                }

                if ((position.GetColorAt(location) == PieceColor.WHITE
                    && (mask & pinnedKingLocation) != 0x0000000000000000UL
                    && (mask & pinMask) != 0x0000000000000000UL)
                    || (position.GetColorAt(location) == PieceColor.BLACK
                    && (mask & pinnedKingLocation) != 0x0000000000000000UL
                    && (mask & pinMask) != 0x0000000000000000UL))
                {

                    occupancy = (position.WhitePieces | position.BlackPieces) & mask;

                    if (pinnedKingLocation < location)
                    {
                        goUp = true;

                        if (offset == 9)
                        {
                            guard = 0x007F7F7F7F7F7F7FUL;
                        }
                        else if (offset == 1)
                        {
                            guard = 0x7F7F7F7F7F7F7F7FUL;
                        }
                        else if (offset == 8)
                        {
                            guard = 0x00FFFFFFFFFFFFFFUL;
                        }
                        else if (offset == 7)
                        {
                            guard = 0x00FEFEFEFEFEFEFEUL;
                        }
                        else
                        {
                            guard = 0x0000000000000000UL;
                        }
                    }
                    else
                    {
                        goUp = false;

                        if (offset == 9)
                        {
                            guard = 0xFEFEFEFEFEFEFE00UL;
                        }
                        else if (offset == 1)
                        {
                            guard = 0xFEFEFEFEFEFEFEFEUL;
                        }
                        else if (offset == 8)
                        {
                            guard = 0xFFFFFFFFFFFFFF00UL;
                        }
                        else if (offset == 7)
                        {
                            guard = 0x7F7F7F7F7F7F7F00UL;
                        }
                        else
                        {
                            guard = 0x0000000000000000UL;
                        }
                    }

                    if ((pinnedKingLocation & guard) != 0x0000000000000000UL)
                    {
                        tmpLocation = goUp ? pinnedKingLocation << offset : pinnedKingLocation >> offset;

                        while (tmpLocation != location)
                        {
                            if ((tmpLocation & occupancy) != 0x0000000000000000UL)
                            {
                                result = false;
                                break;
                            }

                            tmpLocation = goUp ? tmpLocation << offset : tmpLocation >> offset;
                        }

                        if (result && (tmpLocation & guard) != 0x0000000000000000UL)
                        {
                            tmpLocation = goUp ? tmpLocation << offset : tmpLocation >> offset;

                            while (true)
                            {
                                if ((tmpLocation & occupancy) != 0x0000000000000000UL)
                                {
                                    if ((tmpLocation & pinMask) == 0x0000000000000000UL)
                                    {
                                        result = false;
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    if ((tmpLocation & guard) != 0x0000000000000000UL)
                                    {
                                        tmpLocation = goUp ? tmpLocation << offset : tmpLocation >> offset;
                                    }
                                    else
                                    {
                                        result = false;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        private void GenerateSlidingMovesAt(ulong location, ref Position position, UInt16 direction, bool evade)
        {
            int locationIndex;
            ulong moveMask = 0x0000000000000000UL;
            ulong currentMove;
            PieceColor color = position.GetColorAt(location).Value;
            ulong opposingPieces = color == PieceColor.WHITE ? position.BlackPieces : position.WhitePieces;
            ulong ownPieces = color == PieceColor.WHITE ? position.WhitePieces : position.BlackPieces;
            ulong blocker;
            int blockerIndex;

            if (util.PopCount(location) == 1)
            {
                locationIndex = util.GetLSB(location);

                if (direction == 0x01)
                {
                    moveMask = RayAttacksN[locationIndex];
                    blockerIndex = util.GetLSB(moveMask & (position.WhitePieces | position.BlackPieces));

                    if (blockerIndex != -1)
                    {
                        blocker = 0x0000000000000001UL << blockerIndex;
                        moveMask = moveMask & ~RayAttacksN[blockerIndex];
                        moveMask = moveMask & ~ownPieces;
                    }
                }
                else if (direction == 0x02)
                {
                    moveMask = RayAttacksNE[locationIndex];
                    blockerIndex = util.GetLSB(moveMask & (position.WhitePieces | position.BlackPieces));

                    if (blockerIndex != -1)
                    {
                        blocker = 0x0000000000000001UL << blockerIndex;
                        moveMask = moveMask & ~RayAttacksNE[blockerIndex];
                        moveMask = moveMask & ~ownPieces;
                    }
                }
                else if (direction == 0x04)
                {
                    moveMask = RayAttacksE[locationIndex];
                    blockerIndex = util.GetLSB(moveMask & (position.WhitePieces | position.BlackPieces));

                    if (blockerIndex != -1)
                    {
                        blocker = 0x0000000000000001UL << blockerIndex;
                        moveMask = moveMask & ~RayAttacksE[blockerIndex];
                        moveMask = moveMask & ~ownPieces;
                    }
                }
                else if (direction == 0x08)
                {
                    moveMask = RayAttacksSE[locationIndex];
                    blockerIndex = util.GetMSB(moveMask & (position.WhitePieces | position.BlackPieces));

                    if (blockerIndex != -1)
                    {
                        blocker = 0x0000000000000001UL << blockerIndex;
                        moveMask = moveMask & ~RayAttacksSE[blockerIndex];
                        moveMask = moveMask & ~ownPieces;
                    }
                }
                else if (direction == 0x10)
                {
                    moveMask = RayAttacksS[locationIndex];
                    blockerIndex = util.GetMSB(moveMask & (position.WhitePieces | position.BlackPieces));

                    if (blockerIndex != -1)
                    {
                        blocker = 0x0000000000000001UL << blockerIndex;
                        moveMask = moveMask & ~RayAttacksS[blockerIndex];
                        moveMask = moveMask & ~ownPieces;
                    }
                }
                else if (direction == 0x20)
                {
                    moveMask = RayAttacksSW[locationIndex];
                    blockerIndex = util.GetMSB(moveMask & (position.WhitePieces | position.BlackPieces));

                    if (blockerIndex != -1)
                    {
                        blocker = 0x0000000000000001UL << blockerIndex;
                        moveMask = moveMask & ~RayAttacksSW[blockerIndex];
                        moveMask = moveMask & ~ownPieces;
                    }
                }
                else if (direction == 0x40)
                {
                    moveMask = RayAttacksW[locationIndex];
                    blockerIndex = util.GetMSB(moveMask & (position.WhitePieces | position.BlackPieces));

                    if (blockerIndex != -1)
                    {
                        blocker = 0x0000000000000001UL << blockerIndex;
                        moveMask = moveMask & ~RayAttacksW[blockerIndex];
                        moveMask = moveMask & ~ownPieces;
                    }
                }
                else if (direction == 0x80)
                {
                    moveMask = RayAttacksNW[locationIndex];
                    blockerIndex = util.GetLSB(moveMask & (position.WhitePieces | position.BlackPieces));

                    if (blockerIndex != -1)
                    {
                        blocker = 0x0000000000000001UL << blockerIndex;
                        moveMask = moveMask & ~RayAttacksNW[blockerIndex];
                        moveMask = moveMask & ~ownPieces;
                    }
                }


            }

            while (moveMask != 0x0000000000000000UL)
            {
                currentMove = 0x0000000000000001UL << util.GetLSB(moveMask);
                PieceRank? captured = null;

                if ((opposingPieces & currentMove) != 0x0000000000000000UL)
                {
                    captured = position.GetRankAt(currentMove);
                }

                AddMove(location, currentMove, position.GetRankAt(location).Value,
                                    color, position.HalfMoveCounter, position.CastlingInfo,
                                    position.EnPassantTarget, captured, null, evade, ref position);

                moveMask = moveMask & ~currentMove;
            }
        }

        private void GeneratePawnMovesAt(ulong location, ref Position position, bool evade)
        {
            ulong advancedLocation = position.ColorToMove == PieceColor.WHITE ? location << 8 : location >> 8;
            ulong doubleAdvancedLocation =
                position.ColorToMove == PieceColor.WHITE ? location << 16 : location >> 16;

            ulong captureLeftLocation =
                position.ColorToMove == PieceColor.WHITE ? location << 7 : location >> 7;

            ulong captureRightLocation =
                position.ColorToMove == PieceColor.WHITE ? location << 9 : location >> 9;

            ulong startRankMask =
                position.ColorToMove == PieceColor.WHITE ? 0x000000000000FF00UL : 0x00FF000000000000UL;

            ulong backRankMask =
                position.ColorToMove == PieceColor.WHITE ? 0xFF00000000000000UL : 0x00000000000000FFUL;

            ulong leftEdgeMask =
                position.ColorToMove == PieceColor.WHITE ? 0x0101010101010101UL : 0x8080808080808080UL;

            ulong rightEdgeMask =
                position.ColorToMove == PieceColor.WHITE ? 0x8080808080808080UL : 0x0101010101010101UL;

            ulong opposingPieces =
                position.ColorToMove == PieceColor.WHITE ? position.BlackPieces : position.WhitePieces;

            if (!IsPiecePinnedRank(location, ref position))
            {
                if (!IsPiecePinnedNWSE(location, ref position)
                    && !IsPiecePinnedSWNE(location, ref position))
                {
                    if (((advancedLocation & (position.WhitePieces | position.BlackPieces)) == 0x0000000000000000UL)
                        && ((doubleAdvancedLocation & (position.WhitePieces | position.BlackPieces))
                        == 0x0000000000000000UL)
                        && (location & startRankMask) == location)
                    {
                        AddMove(location, doubleAdvancedLocation, PieceRank.PAWN, position.ColorToMove,
                            position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget, null, null,
                            evade, ref position);
                    }

                    if (((location & backRankMask) == 0x0000000000000000UL)
                        && ((advancedLocation & (position.WhitePieces | position.BlackPieces)) == 0x0000000000000000UL))
                    {
                        if ((advancedLocation & backRankMask) == 0x0000000000000000UL)
                        {
                            AddMove(location, advancedLocation, PieceRank.PAWN, position.ColorToMove,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget, null,
                                null, evade, ref position);
                        }
                        else
                        {
                            AddMove(location, advancedLocation, PieceRank.PAWN, position.ColorToMove,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget, null,
                                PieceRank.QUEEN, evade, ref position);

                            AddMove(location, advancedLocation, PieceRank.PAWN, position.ColorToMove,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget, null,
                                PieceRank.ROOK, evade, ref position);

                            AddMove(location, advancedLocation, PieceRank.PAWN, position.ColorToMove,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget, null,
                                PieceRank.BISHOP, evade, ref position);

                            AddMove(location, advancedLocation, PieceRank.PAWN, position.ColorToMove,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget, null,
                                PieceRank.KNIGHT, evade, ref position);
                        }
                    }
                }

                if (!IsPiecePinnedFile(location, ref position))
                {
                    bool isPinned = false;

                    if (position.ColorToMove == PieceColor.WHITE && IsPiecePinnedSWNE(location, ref position))
                    {
                        isPinned = true;
                    }

                    if (position.ColorToMove == PieceColor.BLACK && IsPiecePinnedSWNE(location, ref position))
                    {
                        isPinned = true;
                    }

                    if (!isPinned)
                    {
                        if (((location & (leftEdgeMask | backRankMask)) == 0x0000000000000000UL)
                            && ((captureLeftLocation & (opposingPieces | position.EnPassantTarget)) != 0x0000000000000000UL))
                        {
                            PieceRank? captured = position.GetRankAt(captureLeftLocation);

                            if ((captureLeftLocation & position.EnPassantTarget) != 0x0000000000000000UL)
                            {
                                evade = true;
                                captured = PieceRank.PAWN;
                            }

                            if ((captureLeftLocation & backRankMask) == 0x0000000000000000UL)
                            {
                                AddMove(location, captureLeftLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, null,
                                        evade, ref position);
                            }
                            else
                            {
                                AddMove(location, captureLeftLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, PieceRank.QUEEN,
                                        evade, ref position);

                                AddMove(location, captureLeftLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, PieceRank.ROOK,
                                        evade, ref position);

                                AddMove(location, captureLeftLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, PieceRank.BISHOP,
                                        evade, ref position);

                                AddMove(location, captureLeftLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, PieceRank.KNIGHT,
                                        evade, ref position);
                            }
                        }
                    }

                    isPinned = false;

                    if (position.ColorToMove == PieceColor.WHITE && IsPiecePinnedNWSE(location, ref position))
                    {
                        isPinned = true;
                    }

                    if (position.ColorToMove == PieceColor.BLACK && IsPiecePinnedNWSE(location, ref position))
                    {
                        isPinned = true;
                    }

                    if (!isPinned)
                    {
                        if (((location & (rightEdgeMask | backRankMask)) == 0x0000000000000000UL)
                            && ((captureRightLocation & (opposingPieces | position.EnPassantTarget)) != 0x0000000000000000UL))
                        {
                            PieceRank? captured = position.GetRankAt(captureRightLocation);

                            if ((captureRightLocation & position.EnPassantTarget) != 0x0000000000000000UL)
                            {
                                evade = true;
                                captured = PieceRank.PAWN;
                            }

                            if ((captureRightLocation & backRankMask) == 0x0000000000000000UL)
                            {
                                AddMove(location, captureRightLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, null,
                                        evade, ref position);
                            }
                            else
                            {
                                AddMove(location, captureRightLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, PieceRank.QUEEN,
                                        evade, ref position);

                                AddMove(location, captureRightLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, PieceRank.ROOK,
                                        evade, ref position);

                                AddMove(location, captureRightLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, PieceRank.BISHOP,
                                        evade, ref position);

                                AddMove(location, captureRightLocation, PieceRank.PAWN,
                                        position.ColorToMove, position.HalfMoveCounter, position.CastlingInfo,
                                        position.EnPassantTarget, captured, PieceRank.KNIGHT,
                                        evade, ref position);
                            }
                        }
                    }
                }
            }
        }

        private void AddMove(ulong src, ulong tgt, PieceRank rank, PieceColor color, int hm, CastlingInfo ci,
            ulong ep, PieceRank? capture, PieceRank? promotion, bool evade, ref Position position)
        {
            if (evade)
            {
                Move m = new Move(src, tgt, rank, color, hm, ci, ep, capture, promotion);
                Move nullMove = new Move(0x0000000000000000UL, 0x0000000000000000UL, PieceRank.PAWN,
                    PieceColor.BLACK, 0, CastlingInfo.NONE, 0x0000000000000000, null, null);

                position.Make(m);
                position.Make(nullMove);

                if (!IsSquareAttacked(color == PieceColor.WHITE ? position.WhiteKing : position.BlackKing, ref position))
                {
                    moveList.pushMove(src, tgt, rank, color, hm, ci, ep, capture, promotion);
                }

                position.Unmake(nullMove);
                position.Unmake(m);
            }
            else
            {
                moveList.pushMove(src, tgt, rank, color, hm, ci, ep, capture, promotion);
            }
        }

        private void GenerateKnightMovesAt(ulong location, ref Position position, bool evade)
        {
            int locationIndex;
            ulong moveMask = 0x0000000000000000UL;
            ulong currentMove;
            PieceColor color = position.GetColorAt(location).Value;
            ulong opposingPieces = color == PieceColor.WHITE ? position.BlackPieces : position.WhitePieces;
            ulong ownPieces = color == PieceColor.WHITE ? position.WhitePieces : position.BlackPieces;


            if (util.PopCount(location) == 1)
            {
                locationIndex = util.GetLSB(location);

                moveMask = KnightAttacks[locationIndex] & ~ownPieces;
            }

            while (moveMask != 0x0000000000000000UL)
            {
                currentMove = 0x0000000000000001UL << util.GetLSB(moveMask);
                PieceRank? captured = null;

                if ((opposingPieces & currentMove) != 0x0000000000000000UL)
                {
                    captured = position.GetRankAt(currentMove);
                }

                AddMove(location, currentMove, PieceRank.KNIGHT, color, position.HalfMoveCounter,
                        position.CastlingInfo, position.EnPassantTarget, captured, null, evade, ref position);

                moveMask = moveMask & ~currentMove;
            }
        }

        private void GenerateKingMovesAt(ulong location, ref Position position, bool evade)
        {
            int locationIndex;
            ulong moveMask = 0x0000000000000000UL;
            ulong currentMove;
            PieceColor color = position.GetColorAt(location).Value;
            ulong opposingPieces = color == PieceColor.WHITE ? position.BlackPieces : position.WhitePieces;
            ulong ownPieces = color == PieceColor.WHITE ? position.WhitePieces : position.BlackPieces;


            if (util.PopCount(location) == 1)
            {
                locationIndex = util.GetLSB(location);

                moveMask = KingAttacks[locationIndex] & ~ownPieces;
            }

            while (moveMask != 0x0000000000000000UL)
            {
                currentMove = 0x0000000000000001UL << util.GetLSB(moveMask);

                if (!IsSquareAttacked(currentMove, ref position))
                {
                    PieceRank? captured = null;

                    if ((opposingPieces & currentMove) != 0x0000000000000000UL)
                    {
                        captured = position.GetRankAt(currentMove).Value;
                    }

                    AddMove(location, currentMove, PieceRank.KING, color, position.HalfMoveCounter,
                        position.CastlingInfo, position.EnPassantTarget, captured, null, evade, ref position);
                }

                moveMask = moveMask & ~currentMove;
            }

            if (color == PieceColor.WHITE)
            {
                if (position.CastlingInfo.HasFlag(CastlingInfo.WHITE_SHORT))
                {
                    if (((position.WhitePieces | position.BlackPieces) & 0x0000000000000060UL)
                        == 0x0000000000000000UL)
                    {
                        if (!IsSquareAttacked(0x0000000000000040UL, ref position)
                            && !IsSquareAttacked(0x0000000000000020UL, ref position)
                            && !IsSquareAttacked(0x0000000000000010UL, ref position))
                        {
                            AddMove(0x0000000000000010UL, 0x0000000000000040UL, PieceRank.KING, color,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget,
                                null, null, evade, ref position);
                        }
                    }
                }

                if (position.CastlingInfo.HasFlag(CastlingInfo.WHITE_LONG))
                {
                    if (((position.WhitePieces | position.BlackPieces) & 0x000000000000000EUL)
                        == 0x0000000000000000UL)
                    {
                        if (!IsSquareAttacked(0x0000000000000008UL, ref position)
                            && !IsSquareAttacked(0x0000000000000004UL, ref position)
                            && !IsSquareAttacked(0x0000000000000010UL, ref position))
                        {
                            AddMove(0x0000000000000010UL, 0x0000000000000004UL, PieceRank.KING, color,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget,
                                null, null, evade, ref position);
                        }
                    }
                }
            }
            else
            {
                if (position.CastlingInfo.HasFlag(CastlingInfo.BLACK_SHORT))
                {
                    if (((position.WhitePieces | position.BlackPieces) & 0x6000000000000000UL)
                        == 0x0000000000000000UL)
                    {
                        if (!IsSquareAttacked(0x4000000000000000UL, ref position)
                            && !IsSquareAttacked(0x2000000000000000UL, ref position)
                            && !IsSquareAttacked(0x1000000000000000UL, ref position))
                        {
                            AddMove(0x1000000000000000UL, 0x4000000000000000UL, PieceRank.KING, color,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget,
                                null, null, evade, ref position);
                        }
                    }
                }

                if (position.CastlingInfo.HasFlag(CastlingInfo.BLACK_LONG))
                {
                    if (((position.WhitePieces | position.BlackPieces) & 0x0E00000000000000UL)
                        == 0x0000000000000000UL)
                    {
                        if (!IsSquareAttacked(0x0800000000000000UL, ref position)
                            && !IsSquareAttacked(0x0400000000000000UL, ref position)
                            && !IsSquareAttacked(0x1000000000000000UL, ref position))
                        {
                            AddMove(0x1000000000000000UL, 0x0400000000000000UL, PieceRank.KING, color,
                                position.HalfMoveCounter, position.CastlingInfo, position.EnPassantTarget,
                                null, null, evade, ref position);
                        }
                    }
                }
            }
        }

        private bool IsSquareAttacked(ulong square, ref Position position)
        {
            bool result = false;
            int squareIndex = util.GetLSB(square);
            int blockerIndex;
            ulong opposingKnights =
                position.ColorToMove == PieceColor.WHITE ? position.BlackKnights : position.WhiteKnights;
            ulong opposingBishops =
                position.ColorToMove == PieceColor.WHITE ? position.BlackBishops : position.WhiteBishops;
            ulong opposingRooks =
                position.ColorToMove == PieceColor.WHITE ? position.BlackRooks : position.WhiteRooks;
            ulong opposingQueens =
                position.ColorToMove == PieceColor.WHITE ? position.BlackQueens : position.WhiteQueens;


            if ((KnightAttacks[squareIndex] & opposingKnights) != 0x0000000000000000UL)
            {
                result = true;
            }

            blockerIndex = util.GetLSB(RayAttacksN[squareIndex] & (position.WhitePieces | position.BlackPieces));

            if (blockerIndex != -1)
            {
                if (((0x0000000000000001UL << blockerIndex) & (opposingRooks | opposingQueens))
                    != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            blockerIndex = util.GetLSB(RayAttacksNE[squareIndex] & (position.WhitePieces | position.BlackPieces));

            if (blockerIndex != -1)
            {
                if (((0x0000000000000001UL << blockerIndex) & (opposingBishops | opposingQueens))
                    != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            blockerIndex = util.GetLSB(RayAttacksE[squareIndex] & (position.WhitePieces | position.BlackPieces));

            if (blockerIndex != -1)
            {
                if (((0x0000000000000001UL << blockerIndex) & (opposingRooks | opposingQueens))
                    != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            blockerIndex = util.GetMSB(RayAttacksSE[squareIndex] & (position.WhitePieces | position.BlackPieces));

            if (blockerIndex != -1)
            {
                if (((0x0000000000000001UL << blockerIndex) & (opposingBishops | opposingQueens))
                    != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            blockerIndex = util.GetMSB(RayAttacksS[squareIndex] & (position.WhitePieces | position.BlackPieces));

            if (blockerIndex != -1)
            {
                if (((0x0000000000000001UL << blockerIndex) & (opposingRooks | opposingQueens))
                    != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            blockerIndex = util.GetMSB(RayAttacksSW[squareIndex] & (position.WhitePieces | position.BlackPieces));

            if (blockerIndex != -1)
            {
                if (((0x0000000000000001UL << blockerIndex) & (opposingBishops | opposingQueens))
                    != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            blockerIndex = util.GetMSB(RayAttacksW[squareIndex] & (position.WhitePieces | position.BlackPieces));

            if (blockerIndex != -1)
            {
                if (((0x0000000000000001UL << blockerIndex) & (opposingRooks | opposingQueens))
                    != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            blockerIndex = util.GetLSB(RayAttacksNW[squareIndex] & (position.WhitePieces | position.BlackPieces));

            if (blockerIndex != -1)
            {
                if (((0x0000000000000001UL << blockerIndex) & (opposingBishops | opposingQueens))
                    != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            if (position.ColorToMove == PieceColor.WHITE)
            {
                if (((((square & 0x00FEFEFEFEFEFEFEUL) << 7) & position.BlackPawns) != 0x0000000000000000UL)
                    || ((((square & 0x007F7F7F7F7F7F7FUL) << 9) & position.BlackPawns) != 0x0000000000000000UL))
                {
                    result = true;
                }

                if ((KingAttacks[squareIndex] & position.BlackKing) != 0x0000000000000000UL)
                {
                    result = true;
                }
            }
            else
            {
                if (((((square & 0xFEFEFEFEFEFEFE00UL) >> 9) & position.WhitePawns) != 0x0000000000000000UL)
                    || ((((square & 0x7F7F7F7F7F7F7F00UL) >> 7) & position.WhitePawns) != 0x0000000000000000UL))
                {
                    result = true;
                }

                if ((KingAttacks[squareIndex] & position.WhiteKing) != 0x0000000000000000UL)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}