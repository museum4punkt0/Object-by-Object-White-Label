/**
 * Created by Willy
 */

using System.Collections;
using UnityEngine;

namespace Utils
{
	public class CoroutineUtils : Singleton<CoroutineUtils>
	{
		public Coroutine _StartCoroutine(IEnumerator iEnumerator, bool stopBeforeStart = false, Coroutine coroutineToStop = null)
		{
			Coroutine coroutine = null;

			if (iEnumerator != null)
			{
				if (stopBeforeStart && coroutineToStop != null) coroutineToStop = _StopCoroutine(coroutineToStop);
				coroutine = StartCoroutine(iEnumerator);
			}

			return coroutine;
		}

		public Coroutine _StopCoroutine(Coroutine coroutine)
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
				coroutine = null;
			}

			return coroutine;
		}
	}
}
