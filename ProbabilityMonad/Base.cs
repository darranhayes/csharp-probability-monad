﻿using System.Collections.Generic;
using System.Linq;
using ProbabilityMonad;
using static ProbabilityMonad.FiniteExtensions;
using System;

namespace ProbabilityMonad
{
    /// <summary>
    /// Export constructors
    /// </summary>
    public static class Base
    {
        public static Random Gen = new Random();
        /// <summary>
        /// Probability constructor
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        public static Prob Prob(double probability)
        {
            return new LogProb(Math.Log(probability));
        }

        /// <summary>
        /// ItemProb constructor 
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="item"></param>
        /// <param name="prob"></param>
        /// <returns></returns>
        public static ItemProb<A> ItemProb<A>(A item, Prob prob)
        {
            return new ItemProb<A>(item, prob);
        }

        /// <summary>
        /// Uniform distribution over list of items
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static FiniteDist<A> EnumUniformD<A>(IEnumerable<A> items)
        {
            var uniform = items.Select(i => new ItemProb<A>(i, Prob(1)));
            return new FiniteDist<A>(Normalize(uniform));
        }

        /// <summary>
        /// Uniform distribution, using parameters
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static FiniteDist<A> UniformD<A>(params A[] items)
        {
            var itemList = new List<A>(items);
            return EnumUniformD(itemList);
        }

        /// <summary>
        /// Bernoulli distribution constructed from success probability
        /// </summary>
        /// <param name="prob"></param>
        /// <returns></returns>
        public static FiniteDist<bool> Bernoulli(Prob prob)
        {
            return new FiniteDist<bool>(ItemProb(true, prob), ItemProb(false, Prob(1 - prob.Value)));
        }


        /// <summary>
        /// Normal dist constructor
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="variance"></param>
        /// <returns></returns>
        public static Normal Normal(double mean, double variance)
        {
            return new Normal(mean, variance, Gen);
        }

        /// <summary>
        /// Primitive constructor
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static Dist<A> Primitive<A>(ContDist<A> dist)
        {
            return new Primitive<A>(dist);
        }

        /// <summary>
        /// Pure constructor, monadic return
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dist<A> Return<A>(A value)
        {
            return new Pure<A>(value);
        }

        /// <summary>
        /// Conditional constructor
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="likelihood"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static Dist<A> Condition<A>(Func<A, Prob> likelihood, Dist<A> dist)
        {
            return new Conditional<A>(likelihood, dist);
        }

        /// <summary>
        /// Beta constructoe
        /// </summary>
        public static Beta Beta(double alpha, double beta)
        {
            return new Beta(beta, alpha, Gen);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Prob Pdf(ContDist<double> dist, double y)
        {
            if (dist is Normal)
            {
                var normal = dist as Normal;
                return Prob(MathNet.Numerics.Distributions.Normal.PDF(normal.Mean, normal.Variance, y));
            }
            if (dist is Beta)
            {
                var beta = dist as Beta;
                return Prob(MathNet.Numerics.Distributions.Beta.PDF(beta.alpha, beta.beta, y));
            }
            throw new NotImplementedException("No PDF for this distribution implemented");
        }
    }
}
