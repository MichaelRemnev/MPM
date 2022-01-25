(defn add [first_symbol, alph]
  (reduce (fn [result, letter] (if (str/starts-with? letter, first_symbol)
                              result
                              (conj result, (str first_symbol, letter))))
          []
          alph))

(defn ads [symbols, alph]
  (reduce into (map #(add % alph) symbols)))


(defn task1 [symbols, n]
  (reduce (fn [alph _] (ads symbols, alph)) symbols (range 1, n)))

(task1 ["a", "b", "c"], 3)