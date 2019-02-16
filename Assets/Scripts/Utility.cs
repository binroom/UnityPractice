using System.Collections;

public static class Utility {
	public static T[] ShuffleArray<T> (T[] array, int seed) {
		System.Random randomGenerator = new System.Random (seed);
		int arrayLen = array.Length;
		for (int i = 0; i < arrayLen - 1; i++) {
			int randomIndex = randomGenerator.Next (i, arrayLen);
			T tempItem = array[randomIndex];

			array[randomIndex] = array[i];
			array[i] = tempItem;
		}

		return array;
	}
}