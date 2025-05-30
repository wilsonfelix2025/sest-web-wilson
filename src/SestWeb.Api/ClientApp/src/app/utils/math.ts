export class MathUtils {
  /**
   * Interpolates the value of x for a given y, based on two other points,
   * (x0, y0) and (x1, y1).
   *
   * @param x0 x coordinate of the previous point.
   * @param y0 y coordinate of the previous point.
   * @param x1 x coordinate of the next point.
   * @param y1 y coordinate of the next point.
   * @param y current y coordinate.
   */
  static linearInterpolate(x0: number, y0: number, x1: number, y1: number, y: number) {
    /**
     * This method uses linear interpolation; if you don't understand what
     * it is, you shouldn't change it.
     */
    return ((y - y0) * (x1 - x0) / (y1 - y0)) + x0;
  }
}
