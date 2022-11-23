import React from "react"
import { SpinnerCircularSplit } from "spinners-react"

export const Loading = (props) => {
    return <SpinnerCircularSplit {...props} size={150} thickness={110} speed={70} color="rgba(27, 39, 55, 1)" secondaryColor="rgba(224, 61, 21, 1)" />
}    