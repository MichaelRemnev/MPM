(defn add [first_symbol, words]
  (loop [first_word (first words) tail (rest words)  result []]
    (if (empty? first_word)
      result
      (if (str/starts-with? first_word, first_symbol)
        (recur (first tail), (rest tail), result) ; then
        (recur (first tail), (rest tail), (conj result, (str first_symbol, first_word))))))) ; else

(defn ads [symbols, words]
  (loop [first_symbol (first symbols), symbols (rest symbols), result []]
    (if (empty? first_symbol)
      result ; then
      (recur (first symbols), (rest symbols), (concat result (add first_symbol, words)))))) ; else

(defn task1 [symbols, n]
  (loop [n, (dec n), words, symbols]
    (if (> n, 0)
      (recur (dec n), (ads symbols, words)) ; then
      words))) ; else

(task1 ["a", "b", "c"], 3)