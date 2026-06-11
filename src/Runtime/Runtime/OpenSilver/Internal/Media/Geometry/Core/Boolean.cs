// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of Boolean.h / Boolean.cpp

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// Builds result figures from scanner junction processing.
/// Overrides CScanner.ProcessTheJunction to pair chains into pre-figures
/// and assemble them into closed figures on the result shape.
/// </summary>
internal class COutline : CScanner
{
    /// <summary>
    /// A list of chains that will eventually form a figure.
    /// </summary>
    private sealed class CPreFigure
    {
        internal CChain First;
        internal CChain Last;

        internal void Initialize(CChain first, CChain last)
        {
            AssumeAsFirst(first);
            AssumeAsLast(last);
        }

        internal void Assume(CPreFigure other)
        {
            AssumeAsLast(other.Last);
            other.First = other.Last = null;
        }

        internal void AddToShape(COutline outline)
        {
            Debug.Assert(First != null);

            outline.StartFigureFromChain(First);

            var chain = First;
            while (chain != null)
            {
                outline.AddChainToFigure(chain);
                chain = GetNextChain(chain);
            }

            First = Last = null;
        }

        internal void AssumeAsFirst(CChain chain)
        {
            chain.TaskData = this;
            First = chain;
        }

        internal void AssumeAsLast(CChain chain)
        {
            chain.TaskData = this;
            Last = chain;
        }

        internal static CChain GetNextChain(CChain chain)
        {
            Debug.Assert(chain != null);
            return chain.TaskData2 as CChain;
        }
    }

    private sealed class CPreFigurePool
    {
        private readonly Stack<CPreFigure> _pool = [];

        internal CPreFigure AllocatePreFigure(CChain first, CChain last)
        {
            var pNew = _pool.Count > 0 ? _pool.Pop() : new CPreFigure();
            pNew.Initialize(first, last);
            return pNew;
        }

        internal void Free(CPreFigure item) => _pool.Push(item);
    }

    private readonly CPreFigurePool _preFigurePool = new();

    protected CapacityStreamGeometryContext _context;
    protected bool _figureOpen;
    protected bool _retrieveCurves;

    // Bezier reconstruction state
    protected CBezierFragment _curve;
    protected bool _segmentReversed;
    protected bool _curveReversed;
    protected bool _downwardTraversal;
    protected CVertex _currentCurveVertex;

    internal COutline(CapacityStreamGeometryContext context, bool fRetrieveCurves = true, double tolerance = 0)
        : base(tolerance)
    {
        _context = context;
        _figureOpen = false;
        _retrieveCurves = fRetrieveCurves;
        m_fCachingCurves = fRetrieveCurves;
        _currentCurveVertex = null;
    }

    internal override void ProcessTheJunction()
    {
        bool fOdd;
        var pLeftmostHead = m_oJunction.GetLeftmostHead(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
        var pRightmostHead = m_oJunction.GetRightmostHead(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
        var pLeftmostTail = m_oJunction.GetLeftmostTail(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
        var pRightmostTail = m_oJunction.GetRightmostTail(CChain.CHAIN_REDUNDANT_OR_CANCELLED);

        if (pLeftmostHead == null && pLeftmostTail == null)
        {
            return;
        }

        if (pLeftmostHead != null)
        {
            if (pRightmostHead == null)
            {
                throw new InvalidOperationException("Scanner inconsistency");
            }

            if (!pLeftmostHead.IsSideRight())
            {
                AppendPairs(pLeftmostHead, pRightmostHead, pLeftmostTail, pRightmostTail);
            }
            else
            {
                if (pLeftmostTail != null)
                {
                    Append(pLeftmostHead, pLeftmostTail, false);

                    ResetLeft(ref pLeftmostHead, ref pRightmostHead);
                    ResetLeft(ref pLeftmostTail, ref pRightmostTail);
                    if (pLeftmostHead != null || pLeftmostTail != null)
                    {
                        AppendPairs(pLeftmostHead, pRightmostHead, pLeftmostTail, pRightmostTail);
                    }
                }
                else
                {
                    StartPreFigure(pLeftmostHead, pRightmostHead);

                    ResetBoth(ref pLeftmostHead, ref pRightmostHead);
                    if (pLeftmostHead != null)
                    {
                        AppendHeadPairs(pLeftmostHead, pRightmostHead, out fOdd);
                        if (fOdd)
                        {
                            throw new InvalidOperationException("Scanner inconsistency: odd head count");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Assert(pLeftmostTail != null);
            Debug.Assert(pRightmostTail != null);

            if (pLeftmostTail.IsSideRight())
            {
                AppendTails(pRightmostTail, pLeftmostTail);

                ResetBoth(ref pLeftmostTail, ref pRightmostTail);
                if (pLeftmostTail != null)
                {
                    AppendTailPairs(pLeftmostTail, pRightmostTail, out fOdd);
                    if (fOdd)
                    {
                        throw new InvalidOperationException("Scanner inconsistency: odd tail count");
                    }
                }
            }
            else
            {
                AppendTailPairs(pLeftmostTail, pRightmostTail, out fOdd);
                if (fOdd)
                {
                    throw new InvalidOperationException("Scanner inconsistency: odd tail count");
                }
            }
        }
    }

    internal override void ProcessCurrentVertex(CChain chain) { }

    private void ResetLeft(ref CChain pLeftmost, ref CChain pRightmost)
    {
        if (pLeftmost != pRightmost)
        {
            pLeftmost = pLeftmost?.GetRelevantRight(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
        }
        else
        {
            pLeftmost = pRightmost = null;
        }

        if ((pLeftmost == null) != (pRightmost == null))
        {
            throw new InvalidOperationException("Scanner inconsistency");
        }
    }

    private void ResetBoth(ref CChain pLeftmost, ref CChain pRightmost)
    {
        if (pLeftmost?.GetRelevantRight(CChain.CHAIN_REDUNDANT_OR_CANCELLED) != pRightmost)
        {
            pLeftmost = pLeftmost?.GetRelevantRight(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
            if (pLeftmost != null)
            {
                pRightmost = pRightmost?.GetRelevantLeft(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
                if (pRightmost == null)
                {
                    throw new InvalidOperationException("Scanner inconsistency");
                }
            }
            else
            {
                pLeftmost = pRightmost = null;
            }
        }
        else
        {
            pLeftmost = pRightmost = null;
        }
    }

    private void StartPreFigure(CChain first, CChain last)
    {
        Debug.Assert(first != null && last != null);
        if (first == last)
        {
            throw new InvalidOperationException("Scanner inconsistency: single chain prefigure");
        }

        _ = _preFigurePool.AllocatePreFigure(first, last);
        LinkChainTo(first, last);
    }

    private void AppendPairs(CChain pLeftHead, CChain pRightHead, CChain pLeftTail, CChain pRightTail)
    {
        bool fOddHeadCount = false;
        bool fOddTailCount = false;

        if (pLeftHead != null)
        {
            AppendHeadPairs(pLeftHead, pRightHead, out fOddHeadCount);
        }

        if (pLeftTail != null)
        {
            AppendTailPairs(pLeftTail, pRightTail, out fOddTailCount);
        }

        if (fOddHeadCount != fOddTailCount)
        {
            throw new InvalidOperationException("Scanner inconsistency: odd total count");
        }

        if (fOddHeadCount)
        {
            Append(pRightHead, pRightTail, true);
        }
    }

    private void AppendHeadPairs(CChain pLeftmost, CChain pRightmost, out bool fOddCount)
    {
        Debug.Assert(pLeftmost != null && pRightmost != null);

        var pLeft = pLeftmost;
        fOddCount = pLeftmost != null;

        while (pLeft != pRightmost)
        {
            Debug.Assert(pLeft != null);
            if (pLeft.IsSideRight())
            {
                throw new InvalidOperationException("Scanner inconsistency");
            }

            var pRight = pLeft.GetRelevantRight(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
            if (pRight == null || !pRight.IsSideRight())
            {
                throw new InvalidOperationException("Scanner inconsistency");
            }

            StartPreFigure(pRight, pLeft);

            if (pRight == pRightmost)
            {
                fOddCount = false;
                break;
            }

            pLeft = pRight.GetRelevantRight(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
            fOddCount = true;
        }
    }

    private void AppendTailPairs(CChain pLeftmost, CChain pRightmost, out bool fOddCount)
    {
        Debug.Assert(pLeftmost != null && pRightmost != null);

        var pLeft = pLeftmost;
        fOddCount = true;

        while (pLeft != pRightmost)
        {
            Debug.Assert(pLeft != null);
            if (pLeft.IsSideRight())
            {
                throw new InvalidOperationException("Scanner inconsistency");
            }

            var pRight = pLeft.GetRelevantRight(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
            if (pRight == null || !pRight.IsSideRight())
            {
                throw new InvalidOperationException("Scanner inconsistency");
            }

            AppendTails(pLeft, pRight);

            if (pRight == pRightmost)
            {
                fOddCount = false;
                break;
            }

            pLeft = pRight.GetRelevantRight(CChain.CHAIN_REDUNDANT_OR_CANCELLED);
            fOddCount = true;
        }
    }

    private void AppendTails(CChain pLeader, CChain pTrailer)
    {
        Debug.Assert(pLeader != null && pTrailer != null);

        var pLeaderFigure = GetOwnerOf(pLeader);
        var pTrailerFigure = GetOwnerOf(pTrailer);

        if (pLeaderFigure == null || pTrailerFigure == null)
        {
            throw new InvalidOperationException("Scanner inconsistency: unowned tail chain");
        }

        if (pTrailerFigure == pLeaderFigure)
        {
            pLeaderFigure.AddToShape(this);
            CloseFigure();
        }
        else
        {
            LinkChainTo(pLeader, pTrailer);
            pLeaderFigure.Assume(pTrailerFigure);
        }

        _preFigurePool.Free(pTrailerFigure);
    }

    private void Append(CChain pHead, CChain pTail, bool fReverse)
    {
        Debug.Assert(pHead != null && pTail != null);

        if (fReverse)
        {
            LinkChainTo(pTail, pHead);
            GetOwnerOf(pTail)?.AssumeAsLast(pHead);
        }
        else
        {
            LinkChainTo(pHead, pTail);
            GetOwnerOf(pTail)?.AssumeAsFirst(pHead);
        }
    }

    private CPreFigure GetOwnerOf(CChain chain) => chain?.TaskData as CPreFigure;

    private void LinkChainTo(CChain chain, CChain nextChain)
    {
        Debug.Assert(chain != null);
        chain.TaskData2 = nextChain;
    }

    internal void StartFigureFromChain(CChain chain)
    {
        MilPoint2D pt;

        if (chain.IsSideRight())
        {
            pt = chain.GetTailPoint() * m_rInverseScale + m_ptCenter;
        }
        else
        {
            pt = chain.GetHeadPoint() * m_rInverseScale + m_ptCenter;
        }

        // isClosed will be set later via SetClosedState when CloseFigure is called
        _context.BeginFigure(new Point(pt.X, pt.Y), isFilled: true, isClosed: false);
        _figureOpen = true;
    }

    internal void AddChainToFigure(CChain chain)
    {
        Debug.Assert(chain != null);

        if (chain.IsSideRight())
        {
            _downwardTraversal = false;
            _segmentReversed = !chain.IsReversed();

            for (var vt = chain.Tail.Previous; vt != null; vt = vt.Previous)
            {
                AddOutlineVertex(vt);
            }
        }
        else
        {
            _downwardTraversal = true;
            _segmentReversed = chain.IsReversed();

            for (var vt = chain.Head.Next; vt != null; vt = vt.Next)
            {
                AddOutlineVertex(vt);
            }
        }
    }

    private void AddOutlineVertex(CVertex vertex)
    {
        if (_retrieveCurves)
        {
            AddVertexWithCurves(vertex);
        }
        else
        {
            AddVertexSimple(vertex);
        }
    }

    private void AddVertexSimple(CVertex vertex)
    {
        MilPoint2D pt = vertex.Pt * m_rInverseScale + m_ptCenter;

        _context.LineTo(new Point(pt.X, pt.Y), isStroked: true, isSmoothJoin: vertex.SmoothJoin);
    }

    private void AddVertexWithCurves(CVertex vertex)
    {
        var edgeVertex = _downwardTraversal ? vertex : vertex.Next;

        if (edgeVertex != null && edgeVertex.HasCurve())
        {
            AddCurveFragment(edgeVertex.GetCurve(), vertex);
        }
        else
        {
            if (_curve.Assigned())
            {
                FlushCurve();
            }

            AddVertexSimple(vertex);
        }
    }

    private void AddCurveFragment(CBezierFragment fragment, CVertex vertex)
    {
        Debug.Assert(fragment.Assigned());

        if (!_curve.TryExtend(fragment, !_segmentReversed))
        {
            if (_curve.Assigned())
            {
                FlushCurve();
            }

            _curve = fragment;
            _curveReversed = _segmentReversed;
        }

        _currentCurveVertex = vertex;
    }

    private void FlushCurve()
    {
        Debug.Assert(_curve.Assigned());

        bool fNotDegenerate = _curve.ConstructBezier(out CBezier bezier);

        if (fNotDegenerate)
        {
            MilPoint2D pt1 = bezier.ControlPoint1;
            MilPoint2D pt2 = bezier.ControlPoint2;

            if (_curveReversed)
            {
                (pt1, pt2) = (pt2, pt1);
            }

            AddCurveToBezier(pt1, pt2, _currentCurveVertex);
        }

        _curve.Clear();
    }

    private void AddCurveToBezier(in MilPoint2D controlPoint1, in MilPoint2D controlPoint2, CVertex vertex)
    {
        MilPoint2D pt = vertex.Pt * m_rInverseScale + m_ptCenter;

        _context.BezierTo(
            new Point(controlPoint1.X, controlPoint1.Y),
            new Point(controlPoint2.X, controlPoint2.Y),
            new Point(pt.X, pt.Y),
            isStroked: true,
            isSmoothJoin: vertex.SmoothJoin);
    }

    private void CloseFigure()
    {
        if (_retrieveCurves && _curve.Assigned())
        {
            FlushCurve();
        }

        if (_figureOpen)
        {
            _context.SetClosedState(true);
            _figureOpen = false;
        }
    }
}

/// <summary>
/// Classifies chains as left/right/redundant for boolean operations.
/// At each junction, tracks which shape each chain belongs to and applies
/// the combine mode rules.
/// </summary>
internal sealed class CBooleanClassifier : CScanner.CClassifier
{
    private readonly GeometryCombineMode _operation;
    private readonly CScanner.CChain[] _tail = new CScanner.CChain[2];
    private readonly CScanner.CChain[] _left = new CScanner.CChain[2];
    private readonly bool[] _isInside = new bool[2];

    internal CBooleanClassifier(GeometryCombineMode operation)
    {
        _operation = operation;
        _tail[0] = _tail[1] = _left[0] = _left[1] = null;
        _isInside[0] = _isInside[1] = false;
    }

    internal override void Classify(CScanner.CChain pLeftmostTail, CScanner.CChain pLeftmostHead, CScanner.CChain pLeft)
    {
        Debug.Assert(pLeftmostHead != null);

        // Identify the first tail in each shape
        _tail[0] = pLeftmostTail;
        while (_tail[0] != null && _tail[0].GetShape() == 1)
        {
            _tail[0] = _tail[0].Right;
        }

        _tail[1] = pLeftmostTail;
        while (_tail[1] != null && _tail[1].GetShape() == 0)
        {
            _tail[1] = _tail[1].Right;
        }

        // Identify the first chain left of the junction in each shape
        _left[0] = pLeft;
        while (_left[0] != null && _left[0].GetShape() == 1)
        {
            _left[0] = _left[0].Left;
        }

        _left[1] = pLeft;
        while (_left[1] != null && _left[1].GetShape() == 0)
        {
            _left[1] = _left[1].Left;
        }

        // Figure out where we are relative to both shapes
        _isInside[0] = _left[0] != null && !_left[0].IsSelfSideRight();
        _isInside[1] = _left[1] != null && !_left[1].IsSelfSideRight();

        // Traverse the junction's heads
        for (var chain = pLeftmostHead; chain != null; chain = chain.Right)
        {
            ClassifyChain(chain);
        }
    }

    private void ClassifyChain(CScanner.CChain chain)
    {
        int which = chain.GetShape();
        Debug.Assert(which == 0 || which == 1);
        int other = 1 - which;

        // Classify the chain in its own shape
        if (_tail[which] != null)
        {
            chain.Continue(_tail[which]);
            _tail[which] = null;
        }
        else
        {
            chain.Classify(_left[which]);
        }

        _isInside[which] = !chain.IsSelfSideRight();
        _left[which] = chain;

        // Mark Boolean operation redundancy
        if (!chain.IsSelfRedundant())
        {
            switch (_operation)
            {
                case GeometryCombineMode.Intersect:
                    if (!_isInside[other])
                    {
                        chain.SetBoolRedundant();
                    }
                    break;

                case GeometryCombineMode.Exclude:
                    if (which == 0)
                    {
                        if (_isInside[1])
                        {
                            chain.SetBoolRedundant();
                        }
                    }
                    else
                    {
                        if (_isInside[0])
                        {
                            chain.FlipBoolSide();
                        }
                        else
                        {
                            chain.SetBoolRedundant();
                        }
                    }
                    break;

                case GeometryCombineMode.Xor:
                    if (_isInside[other])
                    {
                        chain.FlipBoolSide();
                    }
                    break;

                case GeometryCombineMode.Union:
                default:
                    if (_isInside[other])
                    {
                        chain.SetBoolRedundant();
                    }
                    break;
            }
        }
    }
}

/// <summary>
/// The boolean operation scanner. Composes CBooleanClassifier with COutline.
/// </summary>
internal class CBoolean : COutline
{
    private readonly CBooleanClassifier _boolClassifier;

    internal CBoolean(CapacityStreamGeometryContext context, GeometryCombineMode operation, bool fRetrieveCurves = true, double tolerance = 0)
        : base(context, fRetrieveCurves, tolerance)
    {
        _boolClassifier = new CBooleanClassifier(operation);
        m_oJunction.SetClassifier(_boolClassifier);
    }

    internal void SetNext() => m_oChains.SetNext();
}

/// <summary>
/// Classifies 2 shapes as intersecting/overlapping/disjoint
/// </summary>
internal sealed class CRelation : CBoolean
{
    private bool m_fInside0;
    private bool m_fInside1;

    private bool m_fOutside0;
    private bool m_fOutside1;

    private IntersectionDetail m_eResult;

    public CRelation(double rTolerance)
        : base(null, GeometryCombineMode.Intersect, false, rTolerance)
    {
    }

    public IntersectionDetail GetResult()
    {
        if (m_eResult != IntersectionDetail.Intersects)
        {
            if (m_fInside0)
            {
                // Shape[0] has some edges inside.  If it had any edges outside then the result would have
                // been set earlier to Overlap, and we wouldn't be here.  so all the edges of Shape[0] are
                // inside Shape[1], hence:
                Debug.Assert(!m_fOutside0);

                m_eResult = IntersectionDetail.FullyInside;
            }
            else if (m_fInside1)
            {
                // Shape[1] has some edges inside.  If it had any edges outside then the result would have
                // been set earlier to Overlap, and we wouldn't be here.  so all the edges of Shape[1] are
                // inside Shape[0], hence:
                Debug.Assert(!m_fOutside1);
                m_eResult = IntersectionDetail.FullyContains;
            }
            else
            {
                // No shape contains any edge of the other, so they are disjoint
                m_eResult = IntersectionDetail.Empty;
            }
        }

        return m_eResult;
    }

    internal override void ProcessTheJunction()
    {
        CChain pLeftmostHead = m_oJunction.GetLeftmostHead(CChain.CHAIN_SELF_REDUNDANT);
        CChain pChain = pLeftmostHead;

        while (pChain is not null)
        {
            // At this stage the head chains of this junction have been classified for the
            // Intersection Boolean operation.  A chain is therefore BoolRedundant
            // if and only if it lies outside the other shape.

            Debug.Assert(pChain.GetShape() == 0 || pChain.GetShape() == 1);

            if (pChain.IsBoolRedundant())
            {
                // This chain lies outside the other shape
                if (pChain.GetShape() == 0)
                {
                    m_fOutside0 = true;
                }
                else
                {
                    m_fOutside1 = true;
                }
            }
            else
            {
                // This chain lies inside the other shape
                if (pChain.GetShape() == 0)
                {
                    m_fInside0 = true;
                }
                else
                {
                    m_fInside1 = true;
                }
            }

            // See if we can early out
            if ((m_fInside0 && m_fOutside0) || (m_fInside1 && m_fOutside1))
            {
                m_eResult = IntersectionDetail.Intersects;
                m_fDone = true;
                break;
            }

            pChain = pChain.GetRelevantRight(CChain.CHAIN_SELF_REDUNDANT);
        }
    }
}
