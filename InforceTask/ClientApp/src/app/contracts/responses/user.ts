import {TokensPair} from "./tokens-pair";
import {UserResponse} from "./user-response";

export interface User {
  tokens: TokensPair;
  user: UserResponse;
}
