; Infinite sequence of natural numbers
(def naturals (iterate inc 1))

(def base-batch-size 10000)

(defn my-filter-lazy [pred, coll]
  (lazy-seq (when-let [s (seq coll)]
              (if (pred (first s))
                (cons (first s), (my-filter-lazy pred, (rest s)))
                (my-filter-lazy pred, (rest s))))))

(defn my-partition-lazy [n, coll]
  (lazy-seq (when-let [s, (seq coll)]
              (cons (take n, s) (my-partition-lazy n, (drop n s))))))

(defn my-filter-future-finite [pred, thread-number, coll]
  (->>
   coll
   (my-partition-lazy (Math/ceil (/ (count coll), thread-number)))
   (map (fn [elem] (future (doall (my-filter-lazy pred, elem)))))
   (doall)
   (mapcat deref)))

(defn my-filter-lazy-parallel-not-empty [pred, coll, thread-number]
  (->>
   coll
   (my-partition-lazy base-batch-size)
   (map (fn [batch] (my-filter-future-finite pred, thread-number, batch)))
   (apply concat)))


(defn my-filter-lazy-parallel [pred, coll, thread-number] 
  (if (empty? coll) 
    (list) 
    (my-filter-lazy-parallel-not-empty pred, coll, thread-number)))


;; FINITE
;;(doall (filter even? (range 0 4000)))
;;(doall (my-filter-lazy-parallel even? (range 0 4000) 4))

;; INFINITE
(doall (take 400000 (my-filter-lazy-parallel even? naturals, 4)))

;;(time (doall (filter even? (range 0 4000))))
;;(time (doall (my-filter-lazy-parallel even? (range 0 4000) 4)))
;;(time (doall (take 400000 (my-filter-lazy-parallel even? naturals, 4))))