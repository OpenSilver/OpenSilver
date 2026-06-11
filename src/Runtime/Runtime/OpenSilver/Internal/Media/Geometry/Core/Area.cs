// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Port of area.h / area.cpp

using System.Diagnostics;

namespace OpenSilver.Internal.Media.Geometry.Core;

/// <summary>
/// Computes the area of a shape using the sweep-line scanner.
/// </summary>
internal sealed class CArea : CScanner
{
    private double m_rArea;

    internal CArea(double rTolerance)
        : base(rTolerance)
    {
        m_rArea = 0;
    }

    internal override void ProcessCurrentVertex(CChain pChain)
    {
        Debug.Assert(pChain is not null);
        Debug.Assert(pChain.GetPreviousVertex() is not null);

        UpdateWithEdge(pChain, pChain.GetPreviousVertex());
    }

    internal override void ProcessTheJunction()
    {
        CChain pRightMost = m_oJunction.GetRightmostTail(0);

        for (CChain pChain = m_oJunction.GetLeftmostTail(0); pChain is not null; pChain = pChain.Right)
        {
            if (!pChain.IsSelfRedundant())
            {
                if (pChain.IsAtTail())
                {
                    Debug.Assert(pChain.GetPreviousVertex() is not null);

                    UpdateWithEdge(pChain, pChain.GetPreviousVertex());
                }
                else
                {
                    Debug.Assert(pChain.GetCurrentVertex() is not null);
                    Debug.Assert(pChain.GetCurrentVertex().Next == pChain.Tail);

                    UpdateWithEdge(pChain, pChain.GetCurrentVertex());
                }
            }

            if (pChain == pRightMost)
            {
                break;
            }
        }
    }

    internal double GetResult()
    {
        if (m_rArea < 0)
        {
            return 0;
        }
        else
        {
            return m_rArea * m_rInverseScale * m_rInverseScale * 0.5;
        }
    }

    private void UpdateWithEdge(CChain chain, CVertex vertex)
    {
        if (chain.IsSideRight())
        {
            m_rArea -= vertex.GetAreaContribution();
        }
        else
        {
            m_rArea += vertex.GetAreaContribution();
        }
    }
}
