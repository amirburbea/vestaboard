import { Intent } from '@blueprintjs/core';
import { Color } from '@vestaboard-wordle/objects';

export default function getIntent(color?: Color) {
  switch (color) {
    case Color.yellow:
      return Intent.WARNING;
    case Color.green:
      return Intent.SUCCESS;
  }
  return Intent.NONE;
}
