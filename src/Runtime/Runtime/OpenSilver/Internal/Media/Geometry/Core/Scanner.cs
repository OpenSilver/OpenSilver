// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of scanner.h / scanner.cpp

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

internal class CScanner : IPopulationSink
{
    private const int MAX_VERTEX_COUNT = 0xfffe;
    private const int NULL_INDEX = -1;
    private const int SCANNER_LEFT = (int)CLineSegmentIntersection.SIDEINDICATOR.RIGHT;
    private const int SCANNER_INCIDENT = (int)CLineSegmentIntersection.SIDEINDICATOR.INCIDENT;
    private const int SCANNER_RIGHT = (int)CLineSegmentIntersection.SIDEINDICATOR.LEFT;

    private static bool AreAscending(in MilPoint2D ptFirst, in MilPoint2D ptSecond)
    {
        return (ptFirst.Y < ptSecond.Y) || ((ptFirst.Y == ptSecond.Y) && (ptFirst.X < ptSecond.X));
    }

    private static COMPARISON ComparePoints(in MilPoint2D ptFirst, in MilPoint2D ptSecond)
    {
        if (ptFirst.Y < ptSecond.Y)
        {
            return COMPARISON.STRICTLYLESSTHAN;
        }

        if (ptFirst.Y == ptSecond.Y)
        {
            if (ptFirst.X < ptSecond.X)
            {
                return COMPARISON.STRICTLYLESSTHAN;
            }
            if (ptFirst.X == ptSecond.X)
            {
                return COMPARISON.EQUAL;
            }
            return COMPARISON.STRICTLYGREATERTHAN;
        }

        return COMPARISON.STRICTLYGREATERTHAN;
    }

    internal sealed class CVertex
    {
        internal enum EVertexType
        {
            Endpoint,
            Intersection,
            ExactIntersect,
            Unknown
        }

        internal EVertexType Type;
        internal MilPoint2D Pt;
        internal bool SmoothJoin;

        internal CVertex Next;
        internal CVertex Previous;

        internal CVertex SegmentPBase;
        internal CVertex SegmentPTip;

        internal CEdgeIntersection IntersectionInfo = new CEdgeIntersection();
        internal CBezierFragment BezierFragment;

        internal void InitializeAtPoint(in MilPoint2D pt, bool isEndpoint)
        {
            Pt = pt;
            Next = Previous = null;
            SmoothJoin = false;

            if (isEndpoint)
            {
                Type = EVertexType.Endpoint;
                SegmentPTip = null;
            }
            else
            {
                IntersectionInfo.SetEdgeLocation(CLineSegmentIntersection.LOCATION.UNDEFINED);
                Type = EVertexType.ExactIntersect;
                SegmentPBase = null;
            }
        }

        internal void InitializeAtIntersection(CEdgeIntersection edgeIntersect, in MilPoint2D pt)
        {
            Type = EVertexType.Intersection;
            IntersectionInfo.Copy(edgeIntersect);
            IntersectionInfo.SetEdgeLocation(CLineSegmentIntersection.LOCATION.UNDEFINED);
            Pt = pt;
            Next = Previous = null;
            SegmentPTip = null;
            SmoothJoin = false;
        }

        internal void InitializeAsCopy(CVertex other)
        {
            Type = other.Type;
            Pt = other.Pt;
            SmoothJoin = other.SmoothJoin;
            SegmentPBase = other.SegmentPBase;
            SegmentPTip = other.SegmentPTip;
            IntersectionInfo.Copy(other.IntersectionInfo);
            BezierFragment = other.BezierFragment;

            Next = Previous = null;
            ClearCurve();
        }

        internal void InsertAsHead(ref CVertex pHead)
        {
            Next = pHead;
            SegmentPTip = pHead;
            pHead.Previous = this;
            pHead = this;
        }

        internal void InsertAsTail(ref CVertex pTail)
        {
            pTail.Next = this;
            pTail.SegmentPTip = this;
            Previous = pTail;
            Next = null;
            SegmentPTip = null;
            pTail = this;
        }

        internal void LinkEdgeTo(CVertex pNext)
        {
            Next = pNext;
            pNext?.Previous = this;

            CVertex pBase = GetSegmentBase();
            CVertex pVtx = Next;

            while (pVtx != null && !pVtx.IsSegmentEndpoint())
            {
                pVtx.SegmentPBase = pBase;
                pVtx = pVtx.Next;
            }

            if (pVtx != null && pBase != null)
            {
                pBase.SegmentPTip = pVtx;
            }
        }

        internal void Attach(CVertex pHead)
        {
            Next = pHead.Next;
            SegmentPTip = pHead.SegmentPTip;
            Next?.Previous = this;

            CVertex pvi = Next;
            while (pvi != null && !pvi.IsSegmentEndpoint())
            {
                pvi.SegmentPBase = this;
                pvi = pvi.Next;
            }
        }

        internal CVertex GetSegmentBase() => IsSegmentEndpoint() ? this : SegmentPBase;

        internal CVertex GetSegmentTip() => IsSegmentEndpoint() ? SegmentPTip : SegmentPBase?.SegmentPTip;

        internal CVertex GetCrossSegmentBase() => IntersectionInfo.CrossSegmentBase;

        internal MilPoint2D GetApproxCoordinates() => Pt;

        internal MilPoint2D GetExactCoordinates() => Pt;

        internal MilPoint2D GetSegmentBasePoint() => GetSegmentBase().GetExactCoordinates();

        internal MilPoint2D GetSegmentTipPoint() => GetSegmentTip().GetExactCoordinates();

        internal bool IsSegmentEndpoint() => Type == EVertexType.Endpoint;

        internal bool IsExactIntersection() => Type == EVertexType.ExactIntersect;

        internal bool IsExact() => IsSegmentEndpoint() || IsExactIntersection();

        internal void SetCurveInfo(in CBezierFragment fragment) => BezierFragment = fragment;

        internal bool HasCurve() => BezierFragment.Assigned();

        internal CBezierFragment GetCurve() => BezierFragment;

        internal void ClearCurve() => BezierFragment = default;

        internal COMPARISON CompareWith(CVertex pOther)
        {
            if (IsExact())
            {
                MilPoint2D ptThis = GetExactCoordinates();
                if (pOther.IsExact())
                {
                    return ComparePoints(ptThis, pOther.GetExactCoordinates());
                }
                else
                {
                    return ComparisonHelper.Opposite(pOther.IntersectionInfo.CompareWithPoint(ptThis));
                }
            }
            else if (pOther.IsExact())
            {
                return IntersectionInfo.CompareWithPoint(pOther.GetExactCoordinates());
            }
            else
            {
                return IntersectionInfo.CompareWithIntersection(pOther.IntersectionInfo);
            }
        }

        internal bool IsHigherThan(CVertex pOther) => CompareWith(pOther) == COMPARISON.STRICTLYGREATERTHAN;
        internal bool CoincidesWith(CVertex pOther) => CompareWith(pOther) == COMPARISON.EQUAL;

        internal int LocateVertex(CVertex v)
        {
            int location = SCANNER_INCIDENT;

            if (v.IsExact())
            {
                Span<double> c = stackalloc double[2];
                c[0] = v.GetExactCoordinates().X;
                c[1] = v.GetExactCoordinates().Y;

                Span<double> ab = stackalloc double[4];
                ab[0] = GetSegmentBasePoint().X;
                ab[1] = GetSegmentBasePoint().Y;
                ab[2] = GetSegmentTipPoint().X;
                ab[3] = GetSegmentTipPoint().Y;

                location = (int)CLineSegmentIntersection.LocatePointRelativeToLine(c, ab);
            }
            else if (v.GetCrossSegmentBase() != GetSegmentBase())
            {
                Span<double> ab = stackalloc double[4];
                ab[0] = GetSegmentBasePoint().X;
                ab[1] = GetSegmentBasePoint().Y;
                ab[2] = GetSegmentTipPoint().X;
                ab[3] = GetSegmentTipPoint().Y;

                location = (int)v.IntersectionInfo.GetIntersection()
                    .LocateTransverseIntersectionRelativeToLine(ab);
            }

            return location;
        }

        internal void Intersect(CVertex pOther, out bool fIntersect, CIntersectionResult refOnThis, CIntersectionResult refOnOther)
        {
            fIntersect = false;

            if (GetSegmentTip() == null || pOther.GetSegmentTip() == null)
            {
                throw new InvalidOperationException("Scanner failed: edges incomplete.");
            }

            fIntersect = refOnThis.IntersectSegments(
                GetSegmentBase(),
                pOther.GetSegmentBase(),
                out CLineSegmentIntersection.LOCATION eLocationOnAB,
                out CLineSegmentIntersection.LOCATION eLocationOnCD);

            if (!fIntersect)
            {
                return;
            }

            refOnOther.FormDualIntersectionOnCD(refOnThis, GetSegmentBase());

            fIntersect = QueryAndSetEdgeIntersection(eLocationOnAB, refOnThis);
            if (!fIntersect)
            {
                return;
            }

            fIntersect = pOther.QueryAndSetEdgeIntersection(eLocationOnCD, refOnOther);
        }

        internal void IntersectWithSegment(CVertex pSegmentBase, out bool fIntersect, CIntersectionResult result)
        {
            fIntersect = result.IntersectSegments(
                GetSegmentBase(),
                pSegmentBase,
                out CLineSegmentIntersection.LOCATION eLocation,
                out _);

            if (!fIntersect)
            {
                return;
            }

            if (!QueryAndSetEdgeIntersection(eLocation, result))
            {
                throw new InvalidOperationException("Scanner failed: edge intersection not on edge.");
            }
        }

        internal bool QueryAndSetEdgeIntersection(CLineSegmentIntersection.LOCATION eLocation, CIntersectionResult result)
        {
            CVertex pEdgeBase = this;
            CVertex pEdgeTip = Next;

            if (pEdgeBase.IsSegmentEndpoint())
            {
                if (pEdgeTip.IsSegmentEndpoint())
                {
                    result.EdgeLocationOnEdge = eLocation;
                    return true;
                }
                else
                {
                    // EdgeBase == SegmentBase, intersection is below
                    // compare = StrictlyLessThan (intersection is below base)
                }
            }
            else
            {
                COMPARISON compare;
                if (pEdgeBase.IsExact())
                {
                    compare = result.IsExact
                        ? ComparePoints(result.GetExactCoordinates(), pEdgeBase.GetExactCoordinates())
                        : result.CompareWithPoint(pEdgeBase.GetExactCoordinates());
                }
                else
                {
                    compare = result.IsExact
                        ? ComparisonHelper.Opposite(pEdgeBase.IntersectionInfo.CompareWithPoint(result.GetExactCoordinates()))
                        : result.CompareWithSameSegmentIntersection(pEdgeBase.IntersectionInfo);
                }

                if (compare == COMPARISON.STRICTLYGREATERTHAN || compare == COMPARISON.EQUAL)
                {
                    return false;
                }
            }

            // Compare with tip
            if (pEdgeTip.IsSegmentEndpoint() &&
                eLocation == CLineSegmentIntersection.LOCATION.AT_LAST_POINT)
            {
                result.EdgeLocationOnEdge = CLineSegmentIntersection.LOCATION.AT_LAST_POINT;
                return true;
            }

            COMPARISON compareTip;
            if (pEdgeTip.IsExact())
            {
                compareTip = result.IsExact
                    ? ComparePoints(result.GetExactCoordinates(), pEdgeTip.GetExactCoordinates())
                    : result.CompareWithPoint(pEdgeTip.GetExactCoordinates());
            }
            else
            {
                compareTip = result.IsExact
                    ? ComparisonHelper.Opposite(pEdgeTip.IntersectionInfo.CompareWithPoint(result.GetExactCoordinates()))
                    : result.CompareWithSameSegmentIntersection(pEdgeTip.IntersectionInfo);
            }

            if (compareTip == COMPARISON.STRICTLYLESSTHAN)
            {
                return false;
            }

            if (compareTip == COMPARISON.EQUAL)
            {
                result.EdgeLocationOnEdge = CLineSegmentIntersection.LOCATION.AT_LAST_POINT;
                return true;
            }

            result.EdgeLocationOnEdge = CLineSegmentIntersection.LOCATION.ON_OPEN_SEGMENT;
            return true;
        }

        internal MilPoint2D EvalIntersectApproxCoordinates(CEdgeIntersection isect)
        {
            double lambda = isect.GetParameterAlongSegment();
            MilPoint2D ptBase = GetSegmentBasePoint();
            MilPoint2D ptTip = GetSegmentTipPoint();
            return new MilPoint2D(
                ptBase.X + lambda * (ptTip.X - ptBase.X),
                ptBase.Y + lambda * (ptTip.Y - ptBase.Y));
        }
    }

    internal class CEdgeIntersection
    {
        // Accomodate CLineSegmentIntersection segment bias
        internal enum Flavor
        {
            SegmentAB,        // Intersection was evaluated with underlying segment as "ab"
            SegmentCD,        // Intersection was evaluated with underlying segment as "cd"
            SegmentUnknown,   // We don't know yet
        }

        internal Flavor FlavorKind;
        internal CLineSegmentIntersection.LOCATION EdgeLocationOnEdge;
        internal CVertex CrossSegmentBase;
        internal CLineSegmentIntersection Intersection;

        internal void Initialize()
        {
            FlavorKind = Flavor.SegmentUnknown;
            EdgeLocationOnEdge = CLineSegmentIntersection.LOCATION.UNDEFINED;
            CrossSegmentBase = null;
            Intersection = null;
        }

        internal void Copy(CEdgeIntersection other)
        {
            FlavorKind = other.FlavorKind;
            EdgeLocationOnEdge = other.EdgeLocationOnEdge;
            CrossSegmentBase = other.CrossSegmentBase;
            Intersection = other.Intersection;
        }

        internal void SetEdgeLocation(CLineSegmentIntersection.LOCATION loc) => EdgeLocationOnEdge = loc;

        internal CLineSegmentIntersection GetIntersection() => Intersection;

        internal bool IsUnderlyingSegmentAB() => FlavorKind == Flavor.SegmentAB;

        internal double GetParameterAlongSegment()
        {
            return IsUnderlyingSegmentAB()
                ? Intersection.ParameterAlongAB()
                : Intersection.ParameterAlongCD();
        }

        internal COMPARISON CompareWithIntersection(CEdgeIntersection other)
        {
            if (Intersection == other.Intersection)
            {
                return COMPARISON.EQUAL;
            }
            return CLineSegmentIntersection.YXSortTransverseIntersectionPair(Intersection, other.Intersection);
        }

        internal COMPARISON CompareWithPoint(in MilPoint2D pt)
        {
            Span<double> e = stackalloc double[2];
            e[0] = pt.X;
            e[1] = pt.Y;

            return CLineSegmentIntersection.YXSortTransverseIntersectionAndPoint(Intersection, e);
        }

        internal COMPARISON CompareWithSameSegmentIntersection(CEdgeIntersection other)
        {
            CLineSegmentIntersection.PAIRING pairing;
            if (FlavorKind == Flavor.SegmentAB)
            {
                pairing = other.FlavorKind == Flavor.SegmentAB
                    ? CLineSegmentIntersection.PAIRING.FIRST_FIRST
                    : CLineSegmentIntersection.PAIRING.FIRST_LAST;
            }
            else
            {
                pairing = other.FlavorKind == Flavor.SegmentAB
                    ? CLineSegmentIntersection.PAIRING.LAST_FIRST
                    : CLineSegmentIntersection.PAIRING.LAST_LAST;
            }

            return ComparisonHelper.Opposite(
                CLineSegmentIntersection.SortTransverseIntersectionsAlongCommonLineSegment(
                    Intersection, other.Intersection, pairing));
        }
    }

    internal sealed class CIntersectionResult : CEdgeIntersection
    {
        private bool _isExact;
        private MilPoint2D _pt;

        internal CIntersectionResult(CLineSegmentIntersection pIntersection)
        {
            Intersection = pIntersection;
        }

        internal CIntersectionResult(CIntersectionResult other)
        {
            Copy(other);
            _isExact = other._isExact;
            _pt = other._pt;
        }

        internal new CLineSegmentIntersection.LOCATION EdgeLocationOnEdge
        {
            get => base.EdgeLocationOnEdge;
            set => base.EdgeLocationOnEdge = value;
        }

        internal bool IsExact => _isExact;

        internal MilPoint2D GetExactCoordinates() => _pt;

        internal bool IntersectSegments(
            CVertex pABBase,
            CVertex pCDBase,
            out CLineSegmentIntersection.LOCATION eLocationOnAB,
            out CLineSegmentIntersection.LOCATION eLocationOnCD)
        {
            Span<double> ab = stackalloc double[4];
            ab[0] = pABBase.GetSegmentBasePoint().X;
            ab[1] = pABBase.GetSegmentBasePoint().Y;
            ab[2] = pABBase.GetSegmentTipPoint().X;
            ab[3] = pABBase.GetSegmentTipPoint().Y;

            Span<double> cd = stackalloc double[4];
            cd[0] = pCDBase.GetSegmentBasePoint().X;
            cd[1] = pCDBase.GetSegmentBasePoint().Y;
            cd[2] = pCDBase.GetSegmentTipPoint().X;
            cd[3] = pCDBase.GetSegmentTipPoint().Y;

            CLineSegmentIntersection.KIND kind = Intersection.PairwiseIntersect(
                ab, cd, out eLocationOnAB, out eLocationOnCD);

            bool fFound =
                (kind == CLineSegmentIntersection.KIND.TRANSVERSE) &&
                (eLocationOnAB != CLineSegmentIntersection.LOCATION.AT_FIRST_POINT) &&
                (eLocationOnCD != CLineSegmentIntersection.LOCATION.AT_FIRST_POINT);

            if (!fFound)
            {
                return false;
            }

            FlavorKind = Flavor.SegmentAB;
            CrossSegmentBase = pCDBase;

            if (eLocationOnAB == CLineSegmentIntersection.LOCATION.AT_LAST_POINT)
            {
                _isExact = true;
                _pt = pABBase.GetSegmentTipPoint();
            }
            else if (eLocationOnCD == CLineSegmentIntersection.LOCATION.AT_LAST_POINT)
            {
                _isExact = true;
                _pt = pCDBase.GetSegmentTipPoint();
            }
            else
            {
                _isExact = false;
            }

            return true;
        }

        internal void FormDualIntersectionOnCD(CIntersectionResult refOnAB, CVertex pABBase)
        {
            Copy(refOnAB);
            _isExact = refOnAB._isExact;
            _pt = refOnAB._pt;
            FlavorKind = Flavor.SegmentCD;
            CrossSegmentBase = pABBase;
        }

        internal void CopyFrom(CIntersectionResult other)
        {
            Copy(other);
            _isExact = other._isExact;
            _pt = other._pt;
        }
    }

    internal sealed class CIntersectionPool
    {
        private readonly Stack<CLineSegmentIntersection> _pool = [];

        internal CLineSegmentIntersection AllocateIntersection()
        {
            var item = _pool.Count > 0 ? _pool.Pop() : new CLineSegmentIntersection();
            item.Initialize();
            return item;
        }

        internal void Free(CLineSegmentIntersection item) => _pool.Push(item);
    }

    internal sealed class CVertexPool
    {
        private readonly Stack<CVertex> _pool = [];
        internal int VertexCount;
        internal CCurvePool CurvePool;

        internal CVertexPool(CCurvePool curvePool)
        {
            CurvePool = curvePool;
            VertexCount = 0;
        }

        private CVertex AllocateVertex()
        {
            if (VertexCount >= MAX_VERTEX_COUNT)
            {
                throw new InvalidOperationException("Scanner failed: too many vertices.");
            }

            VertexCount++;
            return _pool.Count > 0 ? _pool.Pop() : new CVertex();
        }

        internal CVertex AllocateVertexAtPoint(in MilPoint2D pt, bool isEndpoint)
        {
            var v = AllocateVertex();
            v.InitializeAtPoint(pt, isEndpoint);
            return v;
        }

        internal CVertex AllocateVertexAtIntersection(CEdgeIntersection isect, in MilPoint2D pt)
        {
            var v = AllocateVertex();
            v.InitializeAtIntersection(isect, pt);
            return v;
        }

        internal CVertex CopyVertex(CVertex pvt)
        {
            if (VertexCount >= MAX_VERTEX_COUNT)
            {
                throw new InvalidOperationException("Scanner failed: too many vertices.");
            }

            VertexCount++;
            var v = _pool.Count > 0 ? _pool.Pop() : new CVertex();
            v.InitializeAsCopy(pvt);
            return v;
        }

        internal void Free(CVertex v) => _pool.Push(v);
    }

    internal sealed class CCurvePool
    {
        private CBezier _currentCurve;

        internal void SetNoCurve() => _currentCurve = null;

        internal void AddCurve(in MilPoint2D ptFirst, MilPoint2D pPt1, MilPoint2D pPt2, MilPoint2D pPt3)
        {
            _currentCurve = new CBezier();
            _currentCurve.Initialize(ptFirst, pPt1, pPt2, pPt3);
        }

        internal CBezier GetCurrentCurve() => _currentCurve;
    }

    internal sealed class CChain
    {
        // Chain flag constants
        internal const int CHAIN_REVERSED = 0x0010;
        internal const int CHAIN_COINCIDENT = 0x0020;

        internal const int CHAIN_SIDE_RIGHT = 0x0100;
        internal const int CHAIN_SELF_REDUNDANT = 0x0200;
        internal const int CHAIN_CANCELLED = 0x0400;

        internal const int CHAIN_SHAPE_MASK = 0x0001;
        internal const int CHAIN_BOOL_FLIP_SIDE = 0x1000;
        internal const int CHAIN_BOOL_REDUNDANT = 0x2000;

        internal const int CHAIN_REDUNDANT_MASK = CHAIN_SELF_REDUNDANT | CHAIN_BOOL_REDUNDANT;
        internal const int CHAIN_REDUNDANT_OR_CANCELLED = CHAIN_REDUNDANT_MASK | CHAIN_CANCELLED;
        internal const int CHAIN_SELF_TYPE_MASK = CHAIN_SIDE_RIGHT | CHAIN_SELF_REDUNDANT;
        internal const int CHAIN_INHERITTED_MASK = CHAIN_REVERSED | CHAIN_SHAPE_MASK;

        internal CVertex Head;
        internal CVertex Cursor;
        internal CVertex Tail;

        internal CChain Right;
        internal CChain Left;
        internal CVertexPool VertexPool;
        internal CChainPool ChainPool;
        internal object TaskData;
        internal object TaskData2;

        internal int Flags;
        internal int Winding;
        internal int CandidateHeapIndex = NULL_INDEX;

        private FillRule _fillMode;

        internal void Initialize(CVertexPool vertexPool, CChainPool chainPool, FillRule fillMode, int flags = 0)
        {
            VertexPool = vertexPool;
            ChainPool = chainPool;
            Left = Right = null;
            Cursor = Head = Tail = null;
            Flags = flags;
            Winding = 0;
            TaskData = null;
            TaskData2 = null;
            CandidateHeapIndex = NULL_INDEX;
            _fillMode = fillMode;
        }

        internal void Reset()
        {
            Cursor = Head;
            TaskData = null;
            Left = Right = null;
        }

        internal void StartWith(in MilPoint2D pt)
        {
            Cursor = VertexPool.AllocateVertexAtPoint(pt, true);
            Tail = Head = Cursor;
        }

        internal void StartWithCopyOf(CVertex pVertex)
        {
            Cursor = VertexPool.CopyVertex(pVertex);
            Tail = Head = Cursor;
        }

        internal void InsertVertexAt(in MilPoint2D pt, in CBezierFragment pFragment)
        {
            Cursor = VertexPool.AllocateVertexAtPoint(pt, true);

            if (IsReversed())
            {
                if (pFragment.Assigned())
                {
                    Head.SetCurveInfo(pFragment);
                }
                Cursor.InsertAsHead(ref Head);
            }
            else
            {
                if (pFragment.Assigned())
                {
                    Cursor.SetCurveInfo(pFragment);
                }
                Cursor.InsertAsTail(ref Tail);
            }
        }

        internal void TryAdd(in MilPoint2D ptNew, in CBezierFragment pFragment, out bool fAscending, out bool fAdded)
        {
            fAscending = AreAscending(GetCurrentExactPoint(), ptNew);
            if (Head.Next != null)
            {
                if (fAscending == IsReversed())
                {
                    InsertVertexAt(ptNew, pFragment);
                    fAdded = true;
                }
                else
                {
                    fAdded = false;
                }
            }
            else
            {
                SetReversed(fAscending);
                InsertVertexAt(ptNew, pFragment);
                fAdded = true;
            }
        }

        internal MilPoint2D GetCurrentExactPoint() => Cursor.GetExactCoordinates();
        internal MilPoint2D GetCurrentApproxPoint() => Cursor.GetApproxCoordinates();
        internal MilPoint2D GetCurrentSegmentTipPoint() => Cursor.GetSegmentTipPoint();

        internal MilPoint2D GetCurrentEdgeApproxTipPoint() => Cursor.Next.GetApproxCoordinates();

        internal CVertex GetCurrentVertex() => Cursor;
        internal CVertex GetPreviousVertex() => Cursor.Previous;
        internal CVertex GetCurrentSegmentBase() => Cursor.GetSegmentBase();
        internal CVertex GetCurrentSegmentTip() => Cursor.GetSegmentTip();
        internal CVertex GetCurrentEdgeBase() => Cursor;
        internal CVertex GetCurrentEdgeTip() => Cursor.Next;

        internal bool IsReversed() => (Flags & CHAIN_REVERSED) != 0;

        internal void SetReversed(bool reversed)
        {
            if (reversed)
            {
                Flags |= CHAIN_REVERSED;
            }
            else
            {
                Flags &= ~CHAIN_REVERSED;
            }
        }

        internal void MoveOn() => Cursor = Cursor.Next;

        internal static void LinkLeftRight(CChain pLeft, CChain pRight)
        {
            pLeft?.Right = pRight;
            pRight?.Left = pLeft;
        }

        internal void InsertBetween(CChain pLeft, CChain pRight)
        {
            Left = pLeft;
            Right = pRight;
            pLeft?.Right = this;
            pRight?.Left = this;
        }

        internal int LocateVertex(CVertex pVt) => Cursor.LocateVertex(pVt);

        internal bool IsVertexOnRight(CVertex pvt) => Cursor.LocateVertex(pvt) == SCANNER_RIGHT;

        internal bool IsVertexOnChain(CVertex pvt) => Cursor.LocateVertex(pvt) == SCANNER_INCIDENT;

        internal void Append(CChain pOther)
        {
            CVertex pThisTail = Tail;
            CVertex pOtherHead = pOther.Head;

            pThisTail.Attach(pOtherHead);
            VertexPool.Free(pOtherHead);
            Tail = pOther.Tail;
        }

        internal CChain SplitAtVertex(CVertex pvt)
        {
            if (pvt == Head || pvt.Next == null)
            {
                return null;
            }

            CVertex pCopy = VertexPool.CopyVertex(pvt);

            CChain pSplit = ChainPool.AllocateChain(VertexPool);
            pSplit.Tail = Tail;
            pSplit.Flags = Flags & CHAIN_INHERITTED_MASK;
            pSplit._fillMode = _fillMode;

            pSplit.Head = pSplit.Cursor = pCopy;
            pCopy.LinkEdgeTo(pvt.Next);
            pCopy.SmoothJoin = false;
            pvt.LinkEdgeTo(null);
            Tail = pvt;

            return pSplit;
        }

        internal CChain SplitAtIntersection(CIntersectionResult result)
        {
            CVertex pNextEdge = Cursor.Next;
            if (pNextEdge == null)
            {
                return null;
            }

            if (result.EdgeLocationOnEdge == CLineSegmentIntersection.LOCATION.AT_LAST_POINT)
            {
                if (!IsAtItsLastEdge())
                {
                    return SplitAtVertex(Cursor.Next);
                }
                return null;
            }
            else
            {
                CVertex pvt;
                if (result.IsExact)
                {
                    pvt = VertexPool.AllocateVertexAtPoint(result.GetExactCoordinates(), false);
                }
                else
                {
                    MilPoint2D pt = Cursor.EvalIntersectApproxCoordinates(result);
                    pvt = VertexPool.AllocateVertexAtIntersection(result, pt);
                }

                Cursor.Next.ClearCurve();

                pvt.LinkEdgeTo(Cursor.Next);
                Cursor.LinkEdgeTo(pvt);

                return SplitAtVertex(pvt);
            }
        }

        internal CChain SplitAtCurrentEdgeTip() => SplitAtVertex(Cursor.Next);

        internal CChain SplitAtExactPoint(in MilPoint2D pt)
        {
            CVertex pTip = Cursor.Next;
            if (pTip == null)
            {
                return null;
            }

            CVertex pSplitVertex = VertexPool.AllocateVertexAtPoint(pt, false);

            if (pSplitVertex.CoincidesWith(pTip))
            {
                VertexPool.Free(pSplitVertex);
                if (pTip != Tail)
                {
                    pSplitVertex = pTip;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                pSplitVertex.LinkEdgeTo(Cursor.Next);
                Cursor.LinkEdgeTo(pSplitVertex);
            }

            return SplitAtVertex(pSplitVertex);
        }

        internal CChain SplitAtIncidentVertex(CVertex pVertex, CIntersectionPool pool)
        {
            if (pVertex.IsExact())
            {
                return SplitAtExactPoint(pVertex.GetExactCoordinates());
            }
            else
            {
                var pIntersection = pool.AllocateIntersection();
                var result = new CIntersectionResult(pIntersection);

                Cursor.IntersectWithSegment(pVertex.GetCrossSegmentBase(), out bool fIntersect, result);
                if (!fIntersect)
                {
                    Cursor.IntersectWithSegment(pVertex.GetSegmentBase(), out fIntersect, result);
                    if (!fIntersect)
                    {
                        throw new InvalidOperationException("Scanner failed: incident vertex not intersectable.");
                    }
                }

                return SplitAtIntersection(result);
            }
        }

        internal void ClassifyWinding(CChain pLeft)
        {
            int wLeftWinding = pLeft != null ? pLeft.Winding : 0;
            Winding = IsReversed() ? wLeftWinding - 1 : wLeftWinding + 1;

            if (wLeftWinding != 0)
            {
                if (Winding == 0) SetSideRight();
                else SetRedundant();
            }
            else
            {
                if (Winding == 0) SetRedundant();
            }
        }

        internal void ClassifyAlternate(CChain pLeft)
        {
            if (pLeft != null && !pLeft.IsSelfSideRight())
            {
                SetSideRight();
            }
        }

        internal void ContinueWinding(CChain pChain)
        {
            if (pChain.IsReversed() == IsReversed())
            {
                Winding = pChain.Winding;
                Flags |= (pChain.Flags & CHAIN_SELF_TYPE_MASK);
            }
            else
            {
                Winding = IsReversed() ? pChain.Winding - 2 : pChain.Winding + 2;

                int wType = pChain.Flags & CHAIN_SELF_TYPE_MASK;
                if (wType == CHAIN_SIDE_RIGHT)
                {
                    SetRedundant();
                }
                else if (wType == CHAIN_SELF_REDUNDANT)
                {
                    if (Winding == 0) SetSideRight();
                    else SetRedundant();
                }
            }
        }

        internal void ContinueAlternate(CChain pChain) => Flags |= (pChain.Flags & CHAIN_SELF_TYPE_MASK);

        internal void Classify(CChain pLeft)
        {
            if (_fillMode == FillRule.Nonzero)
            {
                ClassifyWinding(pLeft);
            }
            else
            {
                ClassifyAlternate(pLeft);
            }
        }

        internal void Continue(CChain pChain)
        {
            if (_fillMode == FillRule.Nonzero)
            {
                ContinueWinding(pChain);
            }
            else
            {
                ContinueAlternate(pChain);
            }
        }

        internal CChain GoLeftWhileRedundant(int wRedundantMask)
        {
            CChain pLeft = this;
            while (pLeft != null && pLeft.IsRedundant(wRedundantMask))
            {
                pLeft = pLeft.Left;
            }
            return pLeft;
        }

        internal CChain GoRightWhileRedundant(int wRedundantMask)
        {
            CChain pRight = this;
            while (pRight != null && pRight.IsRedundant(wRedundantMask))
            {
                pRight = pRight.Right;
            }
            return pRight;
        }

        internal CChain GetRelevantLeft(int wRedundantMask) => Left?.GoLeftWhileRedundant(wRedundantMask);

        internal CChain GetRelevantRight(int wRedundantMask) => Right?.GoRightWhileRedundant(wRedundantMask);

        internal bool IsSideRight() => ((Flags & CHAIN_SIDE_RIGHT) == 0) != ((Flags & CHAIN_BOOL_FLIP_SIDE) == 0);

        internal bool IsSelfSideRight() => (Flags & CHAIN_SIDE_RIGHT) != 0;

        internal void SetSideRight() => Flags |= CHAIN_SIDE_RIGHT;

        internal void FlipBoolSide() => Flags ^= CHAIN_BOOL_FLIP_SIDE;

        internal bool IsRedundant(int wRedundantMask) => (Flags & wRedundantMask) != 0;

        internal bool IsSelfRedundant() => (Flags & CHAIN_SELF_REDUNDANT) != 0;

        internal void SetRedundant() => Flags |= CHAIN_SELF_REDUNDANT;

        internal void CancelWith(CChain pOther)
        {
            Flags |= CHAIN_CANCELLED;
            pOther.Flags |= CHAIN_CANCELLED;
        }

        internal void SetCoincidentWithRight() => Flags |= CHAIN_COINCIDENT;
        internal bool CoincidesWithRight() => (Flags & CHAIN_COINCIDENT) != 0;

        internal void SetBoolRedundant() => Flags |= CHAIN_BOOL_REDUNDANT;
        internal bool IsBoolRedundant() => (Flags & CHAIN_BOOL_REDUNDANT) != 0;

        internal int GetShape() => Flags & CHAIN_SHAPE_MASK;

        internal bool IsAtTail() => Cursor == Tail;

        internal bool IsAtItsLastEdge() => GetCurrentEdgeTip() == Tail;

        internal bool IsATailIntersection(CIntersectionResult result) =>
            (result.EdgeLocationOnEdge == CLineSegmentIntersection.LOCATION.AT_LAST_POINT) && IsAtItsLastEdge();

        internal bool CoincidesWith(CChain pOther)
        {
            return Head.Next == Tail &&
                   pOther.Head.Next == pOther.Tail &&
                   Tail.CoincidesWith(pOther.Tail);
        }

        internal void SetCurrentVertexSmooth(bool val) { Cursor.SmoothJoin = val; }

        internal MilPoint2D GetTailPoint() => Tail.GetApproxCoordinates();

        internal MilPoint2D GetHeadPoint() => Head.GetApproxCoordinates();

        internal void Intersect(
            CChain pOther, out bool fIntersect,
            CIntersectionResult isectOnThis, CIntersectionResult isectOnOther)
        {
            Cursor.Intersect(pOther.Cursor, out fIntersect, isectOnThis, isectOnOther);
        }

        internal void IntersectWithSegment(CVertex pOther, out bool fIntersect, CIntersectionResult isect)
        {
            Cursor.IntersectWithSegment(pOther, out fIntersect, isect);
        }

        internal void AssumeTask(CChain pOther)
        {
            TaskData = pOther.TaskData;
            pOther.TaskData = null;
        }
    }

    internal sealed class CChainPool
    {
        private readonly Stack<CChain> _pool = [];
        internal FillRule FillMode;
        internal int ShapeIndex;

        internal CChainPool()
        {
            FillMode = FillRule.Nonzero;
            ShapeIndex = 0;
        }

        internal CChain AllocateChain(CVertexPool vertexPool)
        {
            var chain = _pool.Count > 0 ? _pool.Pop() : new CChain();
            chain.Initialize(vertexPool, this, FillMode, ShapeIndex);
            return chain;
        }

        internal void SetFillMode(FillRule fillMode) => FillMode = fillMode;

        internal void SetNext()
        {
            if (ShapeIndex == 0)
            {
                ShapeIndex = 1;
            }
            else
            {
                throw new InvalidOperationException("Can only operate on 2 shapes.");
            }
        }

        internal void Free(CChain chain) => _pool.Push(chain);
    }

    internal sealed class CMasterHeap : CHeap<CChain>
    {
        internal CMasterHeap()
            : base(false)
        {
        }

        protected override bool IsGreaterThan(CChain a, CChain b) => a.Head.IsHigherThan(b.Head);
    }

    internal sealed class CCandidateHeap : CHeap<CChain>
    {
        internal CCandidateHeap()
            : base(true)
        {
        }

        protected override bool IsGreaterThan(CChain a, CChain b) => a.GetCurrentEdgeTip().IsHigherThan(b.GetCurrentEdgeTip());

        protected override int GetIndex(CChain item) => item.CandidateHeapIndex;

        protected override void SetIndex(CChain item, int index) => item.CandidateHeapIndex = index;
    }

    internal abstract class CHeap<T> where T : class
    {
        private readonly List<T> _items = [];
        private readonly bool _tracksIndex;

        internal CHeap(bool tracksIndex)
        {
            _tracksIndex = tracksIndex;
        }

        protected abstract bool IsGreaterThan(T a, T b);

        protected virtual int GetIndex(T item)
        {
            Debug.Assert(false);
            return -1;
        }

        protected virtual void SetIndex(T item, int index)
        {
            Debug.Assert(false);
        }

        internal int Count => _items.Count;

        internal bool IsEmpty => _items.Count == 0;

        internal void Insert(T item)
        {
            int index = _items.Count;
            _items.Add(item);
            if (_tracksIndex)
            {
                SetIndex(item, index);
            }
            SiftUp(index);
        }

        internal T GetTop() => _items.Count > 0 ? _items[0] : null;

        internal void Pop()
        {
            if (_items.Count == 0)
            {
                return;
            }

            int last = _items.Count - 1;
            Swap(0, last);
            if (_tracksIndex)
            {
                SetIndex(_items[last], -1);
            }
            _items.RemoveAt(last);
            if (_items.Count > 0)
            {
                SiftDown(0);
            }
        }

        internal void Remove(T item)
        {
            int index = _tracksIndex ? GetIndex(item) : _items.IndexOf(item);
            if (index < 0 || index >= _items.Count)
            {
                return;
            }

            int last = _items.Count - 1;
            if (index == last)
            {
                if (_tracksIndex)
                {
                    SetIndex(_items[last], -1);
                }
                _items.RemoveAt(last);
                return;
            }

            Swap(index, last);
            if (_tracksIndex)
            {
                SetIndex(_items[last], -1);
            }
            _items.RemoveAt(last);

            if (index < _items.Count)
            {
                SiftUp(index);
                SiftDown(index);
            }
        }

        internal bool Includes(T item)
        {
            if (_tracksIndex)
            {
                int idx = GetIndex(item);
                return idx >= 0 && idx < _items.Count && ReferenceEquals(_items[idx], item);
            }
            return _items.Contains(item);
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (IsGreaterThan(_items[index], _items[parent]))
                {
                    Swap(index, parent);
                    index = parent;
                }
                else
                {
                    break;
                }
            }
        }

        private void SiftDown(int index)
        {
            int count = _items.Count;
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int largest = index;

                if (left < count && IsGreaterThan(_items[left], _items[largest]))
                {
                    largest = left;
                }

                if (right < count && IsGreaterThan(_items[right], _items[largest]))
                {
                    largest = right;
                }

                if (largest != index)
                {
                    Swap(index, largest);
                    index = largest;
                }
                else
                {
                    break;
                }
            }
        }

        private void Swap(int a, int b)
        {
            (_items[b], _items[a]) = (_items[a], _items[b]);
            if (_tracksIndex)
            {
                SetIndex(_items[a], a);
                SetIndex(_items[b], b);
            }
        }
    }

    internal sealed class CChainList : CFlatteningSink
    {
        internal CChain Current;
        internal CChain FiguresFirstChain;

        internal CCurvePool CurvePool = new CCurvePool();
        internal CVertexPool VertexPool;
        internal CChainPool ChainPool = new CChainPool();

        internal MilPoint2D PtFirst;
        internal MilPoint2D PtCurrent;

        internal CMasterHeap ChainHeap = new CMasterHeap();
        internal double PreviousT;

        internal CChainList()
        {
            VertexPool = new CVertexPool(CurvePool);
            FiguresFirstChain = null;
            Current = null;
        }

        internal void SetFillMode(FillRule fillMode) => ChainPool.SetFillMode(fillMode);

        internal void SetNext() => ChainPool.SetNext();

        internal CChain GetNextChain() => ChainHeap.IsEmpty ? null : ChainHeap.GetTop();

        internal void Pop() => ChainHeap.Pop();

        internal void StartFigure(in MilPoint2D pt)
        {
            PtFirst = PtCurrent = pt;
            Current = FiguresFirstChain = ChainPool.AllocateChain(VertexPool);
            Current.StartWith(PtFirst);
        }

        internal void AddVertex(in MilPoint2D ptNew, CBezierFragment pFragment = default)
        {
            if (ptNew == PtCurrent)
            {
                return;
            }

            Current.TryAdd(ptNew, pFragment, out bool fAscending, out bool fAddedToCurrentChain);

            if (!fAddedToCurrentChain)
            {
                CVertex pLast = Current.GetCurrentVertex();

                if (Current != FiguresFirstChain)
                {
                    Insert(Current);
                }

                Current = ChainPool.AllocateChain(VertexPool);
                Current.StartWithCopyOf(pLast);
                Current.SetReversed(fAscending);
                Current.InsertVertexAt(ptNew, pFragment);
            }

            PtCurrent = ptNew;
        }

        internal override void AcceptPoint(in MilPoint2D ptNew, double t, out bool fAborted)
        {
            var ptRounded = new MilPoint2D(Math.Round(ptNew.X), Math.Round(ptNew.Y));

            if (!IntegerConstants.IsValidInteger30(ptRounded.X) || !IntegerConstants.IsValidInteger30(ptRounded.Y))
            {
                throw new InvalidOperationException("Bad number in scanner.");
            }

            fAborted = false;

            var fragment = new CBezierFragment(CurvePool.GetCurrentCurve(), PreviousT, t);

            AddVertex(ptRounded, fragment);
            PreviousT = t;
        }

        internal void SetCurrentVertexSmooth(bool val) => Current?.SetCurrentVertexSmooth(val);

        internal void EndFigure(in MilPoint2D ptCurrent, bool fClosed)
        {
            if (!fClosed)
            {
                AddVertex(PtFirst);
            }

            if (FiguresFirstChain == Current)
            {
                return;
            }

            if (FiguresFirstChain.IsReversed() == Current.IsReversed())
            {
                if (FiguresFirstChain.IsReversed())
                {
                    FiguresFirstChain.Append(Current);
                    Insert(FiguresFirstChain);
                    ChainPool.Free(Current);
                }
                else
                {
                    Current.Append(FiguresFirstChain);
                    Insert(Current);
                    ChainPool.Free(FiguresFirstChain);
                }
            }
            else
            {
                Insert(FiguresFirstChain);
                Insert(Current);
            }

            FiguresFirstChain = Current = null;
        }

        internal void Insert(CChain pNew)
        {
            ChainHeap.Insert(pNew);
            pNew.Reset();
        }

        internal CBezier GetCurrentCurve() => CurvePool.GetCurrentCurve();

        internal MilPoint2D GetCurrentPoint() => PtCurrent;

        internal void SetNoCurve() => CurvePool.SetNoCurve();

        internal void AddCurve(in MilPoint2D ptFirst, MilPoint2D pPt1, MilPoint2D pPt2, MilPoint2D pPt3)
        {
            PreviousT = 0.0;
            CurvePool.AddCurve(ptFirst, pPt1, pPt2, pPt3);
        }
    }

    internal class CClassifier
    {
        internal virtual void Classify(CChain pLeftmostTail, CChain pLeftmostHead, CChain pLeft)
        {
            CChain pChain = pLeftmostTail;

            if (pChain != null)
            {
                pLeftmostHead.Continue(pChain);
                pLeft = pLeftmostHead;
                pChain = pLeftmostHead.Right;
            }
            else
            {
                pChain = pLeftmostHead;
            }

            while (pChain != null)
            {
                pChain.Classify(pLeft);
                pLeft = pChain;
                pChain = pChain.Right;
            }
        }
    }

    internal sealed class CJunction
    {
        internal CVertex RepVertex;
        internal CChain LeftmostTail;
        internal CChain RightmostTail;
        internal CChain LeftmostHead;
        internal CChain RightmostHead;
        internal CChain JLeft;
        internal CChain JRight;
        internal CScanner Owner;
        internal CClassifier Classifier;
        internal CIntersectionPool IntersectionPool;

        internal CJunction(CClassifier classifier, CIntersectionPool pool)
        {
            IntersectionPool = pool;
            Classifier = classifier;
            Initialize();
        }

        internal void SetOwner(CScanner scanner) => Owner = scanner;

        internal void SetClassifier(CClassifier classifier) => Classifier = classifier;

        internal void Initialize()
        {
            LeftmostHead = RightmostHead = LeftmostTail = RightmostTail = null;
            JLeft = JRight = null;
            RepVertex = null;
        }

        internal bool IsEmpty => LeftmostHead == null && LeftmostTail == null;

        internal MilPoint2D GetPoint() => RepVertex.GetApproxCoordinates();

        internal CChain GetLeftmostHead(int wRedundantMask) => LeftmostHead?.GoRightWhileRedundant(wRedundantMask);

        internal CChain GetRightmostHead(int wRedundantMask) => RightmostHead?.GoLeftWhileRedundant(wRedundantMask);

        internal CChain GetLeftmostTail(int wRedundantMask) => LeftmostTail?.GoRightWhileRedundant(wRedundantMask);

        internal CChain GetRightmostTail(int wRedundantMask) => RightmostTail?.GoLeftWhileRedundant(wRedundantMask);

        internal CChain GetLeft() => JLeft?.GoLeftWhileRedundant(CChain.CHAIN_REDUNDANT_OR_CANCELLED);

        internal CChain GetRight() => JRight?.GoRightWhileRedundant(CChain.CHAIN_REDUNDANT_OR_CANCELLED);

        internal void InsertHead(CChain pNew)
        {
            if (LeftmostHead != null)
            {
                CVertex pSearchKey = pNew.GetCurrentEdgeTip();
                CChain pPrevious = null;
                CChain pCurrent = LeftmostHead;

                while (pPrevious != RightmostHead && pCurrent.IsVertexOnRight(pSearchKey))
                {
                    pPrevious = pCurrent;
                    pCurrent = pCurrent.Right;
                }

                pNew.InsertBetween(pPrevious, pCurrent);

                if (pCurrent == LeftmostHead)
                {
                    LeftmostHead = pNew;
                }
                else if (pPrevious == RightmostHead)
                {
                    RightmostHead = pNew;
                }
            }
            else
            {
                LeftmostHead = RightmostHead = pNew;
            }
        }

        internal void Flush()
        {
            if (LeftmostTail != null)
            {
                Owner.TerminateBatch(LeftmostTail, RightmostTail);
            }

            if (LeftmostHead != null)
            {
                Owner.SplitAtIntersections(LeftmostHead, RightmostHead, JLeft, JRight);
                ClassifyAll();
            }
            else
            {
                Owner.SplitPairAtIntersection(JLeft, JRight);
            }

            Owner.ProcessTheJunction();

            if (LeftmostHead != null)
            {
                Owner.ActivateBatch(LeftmostHead, RightmostHead, JLeft, JRight);
            }

            Initialize();
        }

        internal void ProcessAtHead(CChain pHead, CChain pLeft, CChain pRight, bool fIsOnRightChain)
        {
            LeftmostHead = RightmostHead = pHead;
            RepVertex = pHead.Head;
            JRight = pRight;
            JLeft = pLeft;

            if (fIsOnRightChain)
            {
                RightmostTail = LeftmostTail = pRight;

                while (RightmostTail != null)
                {
                    CChain pNewHead = RightmostTail.SplitAtIncidentVertex(RepVertex, IntersectionPool);
                    if (pNewHead == null)
                    {
                        throw new InvalidOperationException("Scanner failed: expected split at incident vertex.");
                    }

                    InsertHead(pNewHead);
                    if (RightmostTail.CoincidesWithRight())
                    {
                        RightmostTail = RightmostTail.Right;
                    }
                    else
                    {
                        break;
                    }
                }

                JRight = RightmostTail?.Right;
            }

            while (true)
            {
                pHead = Owner.GrabInactiveCoincidentChain(RepVertex);
                if (pHead != null)
                {
                    InsertHead(pHead);
                }
                else
                {
                    break;
                }
            }

            Flush();
        }

        internal void ProcessAtTail(CChain pTail, CChain pLeft, CChain pRight)
        {
            LeftmostTail = RightmostTail = pTail;
            RepVertex = pTail.Tail;

            for (JLeft = pLeft;
                JLeft != null && JLeft.Tail.CoincidesWith(RepVertex);
                JLeft = JLeft.Left)
            {
                LeftmostTail = JLeft;
            }

            for (JRight = pRight;
                JRight != null && JRight.Tail.CoincidesWith(RepVertex);
                JRight = JRight.Right)
            {
                RightmostTail = JRight;
            }

            while (true)
            {
                var pHead2 = Owner.GrabInactiveCoincidentChain(RepVertex);
                if (pHead2 != null)
                {
                    InsertHead(pHead2);
                }
                else
                {
                    break;
                }
            }

            Flush();
        }

        internal void ClassifyAll()
        {
            Classifier.Classify(LeftmostTail, LeftmostHead, JLeft);

            CChain pChain = GetLeftmostHead(CChain.CHAIN_REDUNDANT_MASK);

            while (pChain != null)
            {
                CChain pNext = pChain.GetRelevantRight(CChain.CHAIN_REDUNDANT_MASK);
                if (pNext == null)
                {
                    break;
                }

                if (pChain.CoincidesWith(pNext))
                {
                    pChain.CancelWith(pNext);
                    pChain = pNext.GetRelevantRight(CChain.CHAIN_REDUNDANT_MASK);
                }
                else
                {
                    pChain = pNext;
                }
            }
        }
    }

    internal sealed class CActiveList
    {
        internal CChain Leftmost;

        internal CActiveList()
        {
            Leftmost = null;
        }

        internal bool Locate(CChain pNew, out CChain pLeft, out CChain pRight)
        {
            CVertex pNewHead = pNew.Head;
            bool fIsOnChain = false;
            pLeft = null;
            pRight = Leftmost;

            while (pRight != null)
            {
                int location = pRight.LocateVertex(pNewHead);
                if (location != SCANNER_RIGHT)
                {
                    fIsOnChain = (location == SCANNER_INCIDENT);
                    break;
                }
                pLeft = pRight;
                pRight = pRight.Right;
            }

            return fIsOnChain;
        }

        internal void Insert(CChain pLeft, CChain pRight, CChain pPrevious, CChain pNext)
        {
            CChain.LinkLeftRight(pPrevious, pLeft);
            CChain.LinkLeftRight(pRight, pNext);

            if (pPrevious == null)
            {
                Leftmost = pLeft;
            }
        }

        internal void Remove(CChain pFirst, CChain pLast)
        {
            CChain pPrevious = pFirst.Left;
            CChain pNext = pLast.Right;

            if (pPrevious == null)
            {
                Leftmost = pNext;
            }
            else
            {
                pPrevious.Right = pNext;
            }

            pNext?.Left = pPrevious;

            pFirst.Left = null;
            pLast.Right = null;
        }

        internal void InsertHead(CChain pNew)
        {
            pNew.Left = null;
            pNew.Right = Leftmost;
            Leftmost?.Left = pNew;
            Leftmost = pNew;
        }
    }

    protected CChainList m_oChains;
    protected CActiveList m_oActive = new CActiveList();
    protected CJunction m_oJunction;
    protected CCandidateHeap m_oCandidates = new CCandidateHeap();
    protected CClassifier m_oClassifier = new CClassifier();
    protected CIntersectionPool m_oIntersectionPool = new CIntersectionPool();
    protected MilPoint2D m_ptLastInput;

    protected double m_rTolerance;
    protected MilPoint2D m_ptCenter;
    protected double m_rScale;
    protected double m_rInverseScale;

    protected bool m_fCachingCurves;
    protected bool m_fDone;

    internal CScanner()
        : this(Utils.DEFAULT_FLATTENING_TOLERANCE)
    {
    }

    internal CScanner(double rTolerance)
    {
        m_oChains = new CChainList();
        m_oJunction = new CJunction(m_oClassifier, m_oIntersectionPool);
        m_oJunction.SetOwner(this);
        m_fDone = false;
        m_ptCenter = new MilPoint2D(0.0, 0.0);
        m_fCachingCurves = false;
        m_rTolerance = rTolerance;
        m_rScale = m_rInverseScale = 1.0;
    }

    internal bool SetWorkspaceTransform(Rect bounds)
    {
        double rGeometryExtents = Math.Max(bounds.Width, bounds.Height);

        if (double.IsInfinity(rGeometryExtents) || double.IsNaN(rGeometryExtents))
        {
            throw new InvalidOperationException("Bad number in scanner workspace transform.");
        }

        bool fDegenerate = rGeometryExtents < float.Epsilon;
        if (!fDegenerate)
        {
            m_rScale = IntegerConstants.LARGESTINTEGER26 / rGeometryExtents;
            m_rInverseScale = 1.0 / m_rScale;
            m_ptCenter.X = (bounds.Left + bounds.Right) / 2;
            m_ptCenter.Y = (bounds.Top + bounds.Bottom) / 2;
        }

        return fDegenerate;
    }

    public virtual void SetFillMode(FillRule fillMode) => m_oChains.SetFillMode(fillMode);

    public virtual void StartFigure(in MilPoint2D pt)
    {
        MilPoint2D ptLocal = ConvertToInteger30(pt);
        m_oChains.StartFigure(ptLocal);
        m_ptLastInput = pt;
    }

    public virtual void AddLine(in MilPoint2D ptNew)
    {
        MilPoint2D ptLocal = ConvertToInteger30(ptNew);
        m_oChains.AddVertex(ptLocal);
        m_ptLastInput = ptNew;
    }

    public virtual void AddCurve(in MilPoint2D pt1, in MilPoint2D pt2, in MilPoint2D pt3)
    {
        var flattener = new CBezierFlattener(m_oChains, m_rTolerance * m_rScale);

        if (m_fCachingCurves)
        {
            m_oChains.AddCurve(m_ptLastInput, pt1, pt2, pt3);
        }

        flattener.Point0 = m_oChains.GetCurrentPoint();
        flattener.Point1 = ConvertToInteger30(pt1);
        flattener.Point2 = ConvertToInteger30(pt2);
        flattener.Point3 = ConvertToInteger30(pt3);

        flattener.Flatten(false);
        m_ptLastInput = pt3;
        m_oChains.SetNoCurve();
    }

    public virtual void SetCurrentVertexSmooth(bool val) => m_oChains.SetCurrentVertexSmooth(val);

    public virtual void SetStrokeState(bool val)
    {
        // Intentionally empty – strokes are ignored by the scanner
    }

    public virtual void EndFigure(bool fClosed) => m_oChains.EndFigure(m_oChains.GetCurrentPoint(), fClosed);

    protected MilPoint2D ConvertToInteger30(in MilPoint2D ptIn)
    {
        double x = Math.Round((ptIn.X - m_ptCenter.X) * m_rScale);
        double y = Math.Round((ptIn.Y - m_ptCenter.Y) * m_rScale);

        if (!IntegerConstants.IsValidInteger30(x) || !IntegerConstants.IsValidInteger30(y))
        {
            throw new InvalidOperationException("Bad number in scanner coordinate conversion.");
        }

        return new MilPoint2D(x, y);
    }

    internal void Scan()
    {
        m_fDone = false;
        while (!m_fDone)
        {
            MoveOn();
        }
    }

    protected void MoveOn()
    {
        CChain pTopInactive = m_oChains.GetNextChain();
        CChain pCandidate = m_oCandidates.GetTop();

        if (pTopInactive != null)
        {
            if (pCandidate != null)
            {
                COMPARISON pos = pCandidate.GetCurrentEdgeTip().CompareWith(pTopInactive.Head);
                if (pos == COMPARISON.STRICTLYGREATERTHAN)
                {
                    ProcessCandidate(pCandidate);
                }
                else if (pos == COMPARISON.EQUAL && pCandidate.IsAtItsLastEdge())
                {
                    ProcessCandidate(pCandidate);
                }
                else
                {
                    m_oChains.Pop();
                    Activate(pTopInactive);
                }
            }
            else
            {
                m_oChains.Pop();
                Activate(pTopInactive);
            }
        }
        else if (pCandidate != null)
        {
            ProcessCandidate(pCandidate);
        }
        else
        {
            m_fDone = true;
        }
    }

    protected void ProcessCandidate(CChain pChain)
    {
        if (pChain.IsAtItsLastEdge())
        {
            m_oJunction.ProcessAtTail(pChain, pChain.Left, pChain.Right);
            return;
        }

        m_oCandidates.Pop();
        pChain.MoveOn();

        SplitNeighbor(pChain, pChain.Left, out bool fLeftNeighborSplit);
        if (fLeftNeighborSplit)
        {
            SplitCoincidentChainsLeftOf(pChain.Left);
        }

        SplitNeighbor(pChain, pChain.Right, out bool fRightNeighborSplit);
        if (fRightNeighborSplit)
        {
            SplitCoincidentChainsRightOf(pChain.Right);
        }

        if (!pChain.IsRedundant(CChain.CHAIN_REDUNDANT_MASK))
        {
            ProcessCurrentVertex(pChain);
        }

        InsertCandidate(pChain);
    }

    internal void TerminateBatch(CChain pLeft, CChain pRight)
    {
        m_oActive.Remove(pLeft, pRight);

        CChain pChain = pLeft;
        while (pChain != null)
        {
            m_oCandidates.Remove(pChain);
            if (pChain == pRight)
            {
                break;
            }
            pChain = pChain.Right;
        }
    }

    protected void Activate(CChain pChain)
    {
        bool fJunctionIsOnRightChain = m_oActive.Locate(pChain, out CChain pLeft, out CChain pRight);
        m_oJunction.ProcessAtHead(pChain, pLeft, pRight, fJunctionIsOnRightChain);
    }

    protected void InsertCandidate(CChain pChain) => m_oCandidates.Insert(pChain);

    internal CChain GrabInactiveCoincidentChain(CVertex pV)
    {
        CChain pChain = m_oChains.GetNextChain();
        if (pChain != null)
        {
            if (pV.CoincidesWith(pChain.Head))
            {
                m_oChains.Pop();
            }
            else
            {
                pChain = null;
            }
        }
        return pChain;
    }

    protected void SplitChainAtIntersection(CChain pChain, CIntersectionResult result)
    {
        CChain pSplit = pChain.SplitAtIntersection(result);
        if (pSplit != null)
        {
            m_oChains.Insert(pSplit);
        }
    }

    protected void SplitChainAtCurrentEdgeTip(CChain pChain)
    {
        CChain pSplit = pChain.SplitAtCurrentEdgeTip();
        if (pSplit != null)
        {
            m_oChains.Insert(pSplit);
        }
    }

    protected void SplitChainAtIncidentVertex(CChain pChain, CVertex pVertex)
    {
        CChain pSplit = pChain.SplitAtIncidentVertex(pVertex, m_oIntersectionPool);
        if (pSplit != null)
        {
            m_oChains.Insert(pSplit);
        }
    }

    protected void SplitChainAtSegmentIntersection(CChain pChain, CVertex pSegmentBase)
    {
        var pIntersection = m_oIntersectionPool.AllocateIntersection();
        var result = new CIntersectionResult(pIntersection);

        pChain.IntersectWithSegment(pSegmentBase, out bool fIntersect, result);

        if (fIntersect && !pChain.IsATailIntersection(result))
        {
            SplitChainAtIntersection(pChain, result);
        }
    }

    protected void SplitCandidate(CChain pChain, CIntersectionResult result)
    {
        m_oCandidates.Remove(pChain);
        SplitChainAtIntersection(pChain, result);
        InsertCandidate(pChain);
    }

    protected void SplitNeighbor(CChain pChain, CChain pNeighbor, out bool fSplitNeighbor)
    {
        fSplitNeighbor = false;

        if (pNeighbor == null) return;

        var pIntersection = m_oIntersectionPool.AllocateIntersection();
        var oResultOnChain = new CIntersectionResult(pIntersection);
        var oResultOnNeighbor = new CIntersectionResult(pIntersection);

        bool fSplitChain = false;
        pChain.Intersect(pNeighbor, out bool fIntersect, oResultOnChain, oResultOnNeighbor);
        if (fIntersect)
        {
            fSplitChain = !pChain.IsATailIntersection(oResultOnChain);
            if (fSplitChain)
            {
                SplitChainAtIntersection(pChain, oResultOnChain);
            }

            fSplitNeighbor = !pNeighbor.IsATailIntersection(oResultOnNeighbor);
            if (fSplitNeighbor)
            {
                SplitCandidate(pNeighbor, oResultOnNeighbor);
            }
        }

        if (!fSplitChain && !fSplitNeighbor)
        {
            m_oIntersectionPool.Free(pIntersection);
        }
    }

    protected void SplitCoincidentChainsLeftOf(CChain pChain)
    {
        CChain pLeft = pChain.Left;
        while (pLeft != null && pLeft.CoincidesWithRight())
        {
            m_oCandidates.Remove(pLeft);
            SplitChainAtIncidentVertex(pLeft, pChain.Tail);
            InsertCandidate(pLeft);
            pLeft = pLeft.Left;
        }
    }

    protected void SplitCoincidentChainsRightOf(CChain pChain)
    {
        CChain pRight = pChain;
        while (pRight.CoincidesWithRight())
        {
            pRight = pRight.Right;
            m_oCandidates.Remove(pRight);
            SplitChainAtIncidentVertex(pRight, pChain.Tail);
            InsertCandidate(pRight);
        }
    }

    internal void SplitPairAtIntersection(CChain pLeft, CChain pRight)
    {
        if (pLeft == null || pRight == null)
        {
            return;
        }

        var pIntersection = m_oIntersectionPool.AllocateIntersection();
        var oResultOnLeft = new CIntersectionResult(pIntersection);
        var oResultOnRight = new CIntersectionResult(pIntersection);

        pLeft.Intersect(pRight, out bool fIntersect, oResultOnLeft, oResultOnRight);
        if (fIntersect)
        {
            bool fLeftIsSplit = !pLeft.IsATailIntersection(oResultOnLeft);
            if (fLeftIsSplit)
            {
                SplitCandidate(pLeft, oResultOnLeft);
                SplitCoincidentChainsLeftOf(pLeft);
            }

            bool fRightIsSplit = !pRight.IsATailIntersection(oResultOnRight);
            if (fRightIsSplit)
            {
                SplitCandidate(pRight, oResultOnRight);
                SplitCoincidentChainsRightOf(pRight);
            }

            if (!fLeftIsSplit && !fRightIsSplit)
            {
                m_oIntersectionPool.Free(pIntersection);
            }
        }
    }

    internal void SplitAtIntersections(CChain pLeft, CChain pRight, CChain pPrevious, CChain pNext)
    {
        SplitNeighbor(pLeft, pPrevious, out bool fNeighborWasSplit);
        if (fNeighborWasSplit)
        {
            SplitCoincidentChainsLeftOf(pPrevious);
        }

        SplitNeighbor(pRight, pNext, out fNeighborWasSplit);
        if (fNeighborWasSplit)
        {
            SplitCoincidentChainsRightOf(pNext);
        }

        for (CChain pChain = pLeft; pChain != null && pChain != pRight; pChain = pChain.Right)
        {
            SplitAtCoincidentIntersection(pChain);
        }
    }

    internal void ActivateBatch(CChain pLeft, CChain pRight, CChain pPrevious, CChain pNext)
    {
        m_oActive.Insert(pLeft, pRight, pPrevious, pNext);

        CChain chain = pLeft;
        while (chain != null)
        {
            InsertCandidate(chain);
            if (chain == pRight)
            {
                break;
            }
            chain = chain.Right;
        }
    }

    protected void SplitAtCoincidentIntersection(CChain pChain)
    {
        CChain pRight = pChain.Right;
        if (pRight == null)
        {
            return;
        }

        Span<double> ab = stackalloc double[4];
        ab[0] = pChain.GetCurrentSegmentBase().GetExactCoordinates().X;
        ab[1] = pChain.GetCurrentSegmentBase().GetExactCoordinates().Y;
        ab[2] = pChain.GetCurrentSegmentTip().GetExactCoordinates().X;
        ab[3] = pChain.GetCurrentSegmentTip().GetExactCoordinates().Y;

        Span<double> c = stackalloc double[2];
        c[0] = pRight.GetCurrentSegmentTip().GetExactCoordinates().X;
        c[1] = pRight.GetCurrentSegmentTip().GetExactCoordinates().Y;

        if (CLineSegmentIntersection.LocatePointRelativeToLine(c, ab) == CLineSegmentIntersection.SIDEINDICATOR.INCIDENT)
        {
            COMPARISON compare = pChain.GetCurrentEdgeTip().CompareWith(pRight.GetCurrentEdgeTip());
            if (compare == COMPARISON.STRICTLYGREATERTHAN)
            {
                SplitChainAtCurrentEdgeTip(pChain);
                SplitChainAtIncidentVertex(pRight, pChain.GetCurrentEdgeTip());
            }
            else if (compare == COMPARISON.STRICTLYLESSTHAN)
            {
                SplitChainAtCurrentEdgeTip(pRight);

                CChain pLeft = pChain;
                do
                {
                    SplitChainAtIncidentVertex(pLeft, pRight.Tail);
                    pLeft = pLeft.Left;
                }
                while (pLeft != null && pLeft.CoincidesWithRight());
            }
            else
            {
                SplitChainAtCurrentEdgeTip(pChain);
                SplitChainAtCurrentEdgeTip(pRight);
            }

            pChain.SetCoincidentWithRight();
        }
    }

    internal virtual void ProcessTheJunction()
    {
        // Base implementation does nothing; overridden by COutline/CBoolean
    }

    internal virtual void ProcessCurrentVertex(CChain pChain)
    {
        // Base implementation does nothing; overridden by COutline/CBoolean
    }
}
